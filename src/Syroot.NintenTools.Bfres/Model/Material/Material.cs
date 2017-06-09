using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FMAT subsection of a <see cref="Model"/> subfile, storing information on with which textures and
    /// how technically a surface is drawn.
    /// </summary>
    [DebuggerDisplay(nameof(Material) + " {" + nameof(Name) + "}")]
    public class Material : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{Material}"/> instances.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_name != value)
                {
                    _name = value;
                    NameChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public MaterialFlags Flags { get; set; }

        public INamedResDataList<RenderInfo> RenderInfos { get; private set; }

        public RenderState RenderState { get; private set; }

        public ShaderAssign ShaderAssign { get; private set; }

        public IList<TextureRef> TextureRefs { get; private set; }

        public INamedResDataList<Sampler> Samplers { get; private set; }

        public INamedResDataList<ShaderParam> ShaderParams { get; private set; }

        /// <summary>
        /// Gets the raw data block which stores <see cref="ShaderParam"/> values.
        /// </summary>
        public byte[] ParamData { get; private set; }
        
        /// <summary>
        /// Gets customly attached <see cref="UserData"/> instances.
        /// </summary>
        public INamedResDataList<UserData> UserData { get; private set; }

        public byte[] VolatileFlags { get; private set; }

        // TODO: Methods to access ShaderParam variable values.

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            MaterialHead head = new MaterialHead(loader);
            Name = loader.GetName(head.OfsName);
            Flags = head.Flags;
            RenderInfos = loader.LoadDictList<RenderInfo>(head.OfsRenderInfoDict);
            RenderState = loader.Load<RenderState>(head.OfsRenderState);
            ShaderAssign = loader.Load<ShaderAssign>(head.OfsShaderAssign);
            TextureRefs = loader.LoadList<TextureRef>(head.OfsTextureRefList, head.NumTextureRef);
            Samplers = loader.LoadDictList<Sampler>(head.OfsSamplerDict);
            ShaderParams = loader.LoadDictList<ShaderParam>(head.OfsShaderParamDict);
            loader.Position = head.OfsParamSource;
            ParamData = loader.ReadBytes(head.SizParamSource);
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);

            if (head.OfsVolatileFlag != 0)
            {
                loader.Position = head.OfsVolatileFlag;
                VolatileFlags = loader.ReadBytes((int)Math.Ceiling((float)head.NumShaderParamVolatile / sizeof(byte)));
            }
        }
        
        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="UserData"/> instance.
    /// </summary>
    internal class MaterialHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FMAT";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint OfsName;
        internal MaterialFlags Flags;
        internal ushort Idx;
        internal ushort NumRenderInfo;
        internal byte NumSampler;
        internal byte NumTextureRef;
        internal ushort NumShaderParam;
        internal ushort NumShaderParamVolatile;
        internal ushort SizParamSource;
        internal ushort SizParamRaw;
        internal ushort NumUserData;
        internal uint OfsRenderInfoDict;
        internal uint OfsRenderState;
        internal uint OfsShaderAssign;
        internal uint OfsTextureRefList;
        internal uint OfsSamplerList;
        internal uint OfsSamplerDict;
        internal uint OfsShaderParamList;
        internal uint OfsShaderParamDict;
        internal uint OfsParamSource;
        internal uint OfsUserDataDict;
        internal uint OfsVolatileFlag;
        internal uint UserPointer;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal MaterialHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            OfsName = loader.ReadOffset();
            Flags = loader.ReadEnum<MaterialFlags>(true);
            Idx = loader.ReadUInt16();
            NumRenderInfo = loader.ReadUInt16();
            NumSampler = loader.ReadByte();
            NumTextureRef = loader.ReadByte();
            NumShaderParam = loader.ReadUInt16();
            NumShaderParamVolatile = loader.ReadUInt16();
            SizParamSource = loader.ReadUInt16();
            SizParamRaw = loader.ReadUInt16();
            NumUserData = loader.ReadUInt16();
            OfsRenderInfoDict = loader.ReadOffset();
            OfsRenderState = loader.ReadOffset();
            OfsShaderAssign = loader.ReadOffset();
            OfsTextureRefList = loader.ReadOffset();
            OfsSamplerList = loader.ReadOffset();
            OfsSamplerDict = loader.ReadOffset();
            OfsShaderParamList = loader.ReadOffset();
            OfsShaderParamDict = loader.ReadOffset();
            OfsParamSource = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
            OfsVolatileFlag = loader.ReadOffset();
            UserPointer = loader.ReadUInt32();
        }
    }

    public enum MaterialFlags
    {
        None,
        Visible
    }
}