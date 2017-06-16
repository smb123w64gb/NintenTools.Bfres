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
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FMAT";

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
            loader.CheckSignature(_signature);
            Name = loader.LoadString();
            Flags = loader.ReadEnum<MaterialFlags>(true);
            ushort idx = loader.ReadUInt16();
            ushort numRenderInfo = loader.ReadUInt16();
            byte numSampler = loader.ReadByte();
            byte numTextureRef = loader.ReadByte();
            ushort numShaderParam = loader.ReadUInt16();
            ushort numShaderParamVolatile = loader.ReadUInt16();
            ushort sizParamSource = loader.ReadUInt16();
            ushort sizParamRaw = loader.ReadUInt16();
            ushort numUserData = loader.ReadUInt16();
            RenderInfos = loader.LoadDictList<RenderInfo>();
            RenderState = loader.Load<RenderState>();
            ShaderAssign = loader.Load<ShaderAssign>();
            TextureRefs = loader.LoadList<TextureRef>(numTextureRef);
            uint ofsSamplerList = loader.ReadOffset(); // Only use dict.
            Samplers = loader.LoadDictList<Sampler>();
            uint ofsShaderParamList = loader.ReadOffset(); // Only use dict.
            ShaderParams = loader.LoadDictList<ShaderParam>();
            ParamData = loader.LoadCustom(() => loader.ReadBytes(sizParamSource));
            UserData = loader.LoadDictList<UserData>();
            VolatileFlags = loader.LoadCustom(
                () => loader.ReadBytes((int)Math.Ceiling((float)numShaderParamVolatile / sizeof(byte))));
            uint userPointer = loader.ReadUInt32();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
        }
    }

    public enum MaterialFlags
    {
        None,
        Visible
    }
}