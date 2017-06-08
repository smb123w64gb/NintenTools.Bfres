using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FSHU subfile in a <see cref="ResFile"/>, storing shader parameter animations of a
    /// <see cref="Model"/> instance.
    /// </summary>
    [DebuggerDisplay(nameof(ShaderParamAnim) + " {" + nameof(Name) + "}")]
    public class ShaderParamAnim : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;
        private uint _ofsBindModel;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

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

        public string Path { get; set; }

        public ShaderParamAnimFlags Flags { get; set; }

        public int FrameCount { get; set; }

        public uint BakedSize { get; set; }

        public Model BindModel { get; set; }

        public ushort[] BindIndices { get; set; }

        public IList<ShaderParamMatAnim> ShaderParamMatAnims { get; private set; }

        public INamedResDataList<UserData> UserData { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ShaderParamAnimHead head = new ShaderParamAnimHead(loader);
            Name = loader.GetName(head.OfsName);
            Path = loader.GetName(head.OfsPath);
            Flags = head.Flags;
            FrameCount = head.NumFrame;
            BakedSize = head.SizBaked;
            _ofsBindModel = head.OfsBindModel;

            if (head.OfsBindIndexList != 0)
            {
                loader.Position = head.OfsBindIndexList;
                BindIndices = loader.ReadUInt16s(head.NumMatAnim);
            }

            ShaderParamMatAnims = loader.LoadList<ShaderParamMatAnim>(head.OfsMatAnimList, head.NumMatAnim);
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        void IResData.Reference(ResFileLoader loader)
        {
            BindModel = loader.GetData<Model>(_ofsBindModel);
        }
    }
    
    /// <summary>
    /// Represents the header of a <see cref="ShaderParamAnim"/> instance.
    /// </summary>
    internal class ShaderParamAnimHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSHU";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint OfsName;
        internal uint OfsPath;
        internal ShaderParamAnimFlags Flags;
        internal int NumFrame;
        internal ushort NumMatAnim;
        internal ushort NumUserData;
        internal int NumParamAnim;
        internal int NumCurve;
        internal uint SizBaked;
        internal uint OfsBindModel;
        internal uint OfsBindIndexList;
        internal uint OfsMatAnimList;
        internal uint OfsUserDataDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ShaderParamAnimHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            OfsName = loader.ReadOffset();
            OfsPath = loader.ReadOffset();
            Flags = loader.ReadEnum<ShaderParamAnimFlags>(true);
            NumFrame = loader.ReadInt32();
            NumMatAnim = loader.ReadUInt16();
            NumUserData = loader.ReadUInt16();
            NumParamAnim = loader.ReadInt32();
            NumCurve = loader.ReadInt32();
            SizBaked = loader.ReadUInt32();
            OfsBindModel = loader.ReadOffset();
            OfsBindIndexList = loader.ReadOffset();
            OfsMatAnimList = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
        }
    }

    [Flags]
    public enum ShaderParamAnimFlags : uint
    {
        BakedCurve = 1 << 0,
        Looping = 1 << 2
    }
}