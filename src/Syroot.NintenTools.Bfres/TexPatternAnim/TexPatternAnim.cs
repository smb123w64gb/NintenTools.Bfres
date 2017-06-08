using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FTXP subfile in a <see cref="ResFile"/>, storing texture material pattern animations.
    /// </summary>
    [DebuggerDisplay(nameof(TexPatternAnim) + " {" + nameof(Name) + "}")]
    public class TexPatternAnim : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;
        private uint _ofsBindModel;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// The name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{TexPatternAnim}"/> instances.
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

        public string Path { get; set; }

        public TexPatternAnimFlags Flags { get; set; }

        public int FrameCount { get; set; }

        public uint BakedSize { get; set; }

        public Model BindModel { get; set; }

        public ushort[] BindIndices { get; private set; }

        public IList<TexPatternMatAnim> TexPatternMatAnims { get; private set; }

        public INamedResDataList<TextureRef> TextureRefs { get; private set; }

        public INamedResDataList<UserData> UserData { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            TexPatternAnimHead head = new TexPatternAnimHead(loader);
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

            TexPatternMatAnims = loader.LoadList<TexPatternMatAnim>(head.OfsMatAnimList, head.NumMatAnim);
            TextureRefs = loader.LoadDictList<TextureRef>(head.OfsTextureRefDict);
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        void IResData.Reference(ResFileLoader loader)
        {
            BindModel = loader.GetData<Model>(_ofsBindModel);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="TexPatternAnim"/> instance.
    /// </summary>
    internal class TexPatternAnimHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FTXP";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint OfsName;
        internal uint OfsPath;
        internal TexPatternAnimFlags Flags;
        internal ushort NumUserData;
        internal int NumFrame;
        internal ushort NumTextureRef;
        internal ushort NumMatAnim;
        internal int NumPatAnim;
        internal int NumCurve;
        internal uint SizBaked;
        internal uint OfsBindModel;
        internal uint OfsBindIndexList;
        internal uint OfsMatAnimList;
        internal uint OfsTextureRefDict;
        internal uint OfsUserDataDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal TexPatternAnimHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            OfsName = loader.ReadOffset();
            OfsPath = loader.ReadOffset();
            Flags = loader.ReadEnum<TexPatternAnimFlags>(true);
            NumUserData = loader.ReadUInt16();
            NumFrame = loader.ReadInt32();
            NumTextureRef = loader.ReadUInt16();
            NumMatAnim = loader.ReadUInt16();
            NumPatAnim = loader.ReadInt32();
            NumCurve = loader.ReadInt32();
            SizBaked = loader.ReadUInt32();
            OfsBindModel = loader.ReadOffset();
            OfsBindIndexList = loader.ReadOffset();
            OfsMatAnimList = loader.ReadOffset();
            OfsTextureRefDict = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
        }
    }

    [Flags]
    public enum TexPatternAnimFlags : ushort
    {
        BakedCurve = 1 << 0,
        Looping = 1 << 2
    }
}