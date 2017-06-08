using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FVIS subfile in a <see cref="ResFile"/>, storing visibility animations of <see cref="Bone"/> or
    /// <see cref="Material"/> instances.
    /// </summary>
    [DebuggerDisplay(nameof(VisibilityAnim) + " {" + nameof(Name) + "}")]
    public class VisibilityAnim : INamedResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const ushort _flagsMask = 0b00000000_00000111;
        private const ushort _flagsMaskType = 0b00000001_00000000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;
        private ushort _flags;
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

        public VisibilityAnimFlags Flags
        {
            get { return (VisibilityAnimFlags)(_flags & _flagsMask); }
            set { _flags &= (ushort)(~_flagsMask | (ushort)value); }
        }

        public VisiblityAnimType Type
        {
            get { return (VisiblityAnimType)(_flags & _flagsMaskType); }
            set { _flags &= (ushort)(~_flagsMaskType | (ushort)value); }
        }

        public int FrameCount { get; set; }

        public uint BakedSize { get; set; }

        public Model BindModel { get; set; }

        public ushort[] BindIndices { get; private set; }

        public IList<string> Names { get; private set; }

        public IList<AnimCurve> Curves { get; private set; }

        public byte[] BaseDataList { get; private set; } // Bits controlling visibility.

        public INamedResDataList<UserData> UserData { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            VisibilityAnimHead head = new VisibilityAnimHead(loader);
            Name = loader.GetName(head.OfsName);
            Path = loader.GetName(head.OfsPath);
            _flags = head.Flags;
            FrameCount = head.NumFrame;
            BakedSize = head.SizBaked;
            _ofsBindModel = head.OfsBindModel;

            if (head.OfsBindIndexList != 0)
            {
                loader.Position = head.OfsBindIndexList;
                BindIndices = loader.ReadUInt16s(head.NumAnim);
            }

            if (head.OfsNameList != 0)
            {
                loader.Position = head.OfsNameList;
                Names = loader.GetNames(loader.ReadOffsets(head.NumAnim));
            }

            Curves = loader.LoadList<AnimCurve>(head.OfsCurveList, head.NumCurve);

            if (head.OfsBaseValueList != 0)
            {
                loader.Position = head.OfsBaseValueList;
                BaseDataList = loader.ReadBytes((int)Math.Ceiling(head.NumAnim / 8f));
            }

            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        void IResData.Reference(ResFileLoader loader)
        {
            BindModel = loader.GetData<Model>(_ofsBindModel);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="VisibilityAnim"/> instance.
    /// </summary>
    internal class VisibilityAnimHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FVIS";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint OfsName;
        internal uint OfsPath;
        internal ushort Flags;
        internal ushort NumUserData;
        internal int NumFrame;
        internal ushort NumAnim;
        internal ushort NumCurve;
        internal uint SizBaked;
        internal uint OfsBindModel;
        internal uint OfsBindIndexList;
        internal uint OfsNameList;
        internal uint OfsCurveList;
        internal uint OfsBaseValueList;
        internal uint OfsUserDataDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal VisibilityAnimHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            OfsName = loader.ReadOffset();
            OfsPath = loader.ReadOffset();
            Flags = loader.ReadUInt16();
            NumUserData = loader.ReadUInt16();
            NumFrame = loader.ReadInt32();
            NumAnim = loader.ReadUInt16();
            NumCurve = loader.ReadUInt16();
            SizBaked = loader.ReadUInt32();
            OfsBindModel = loader.ReadOffset();
            OfsBindIndexList = loader.ReadOffset();
            OfsNameList = loader.ReadOffset();
            OfsCurveList = loader.ReadOffset();
            OfsBaseValueList = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
        }
    }

    [Flags]
    public enum VisibilityAnimFlags : ushort
    {
        BakedCurve = 1 << 0,
        Looping = 1 << 2
    }

    public enum VisiblityAnimType : ushort
    {
        Bone,
        Material = 1 << 8
    }
}