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

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{VisibilityAnim}"/> instances.
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

        /// <summary>
        /// Gets or sets the path of the file which originally supplied the data of this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets flags controlling how animation data is stored or how the animation should be played.
        /// </summary>
        public VisibilityAnimFlags Flags
        {
            get { return (VisibilityAnimFlags)(_flags & _flagsMask); }
            set { _flags &= (ushort)(~_flagsMask | (ushort)value); }
        }

        /// <summary>
        /// Gets or sets the kind of data the animation controls.
        /// </summary>
        public VisibilityAnimType Type
        {
            get { return (VisibilityAnimType)(_flags & _flagsMaskType); }
            set { _flags &= (ushort)(~_flagsMaskType | (ushort)value); }
        }

        /// <summary>
        /// Gets or sets the total number of frames this animation plays.
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes required to bake all <see cref="Curves"/>.
        /// </summary>
        public uint BakedSize { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Model"/> instance affected by this animation.
        /// </summary>
        public Model BindModel { get; set; }

        /// <summary>
        /// Gets the indices of entries in the <see cref="Skeleton.Bones"/> or <see cref="Model.Materials"/>
        /// dictionaries to bind to for each animation. <see cref="UInt16.MaxValue"/> specifies no binding.
        /// </summary>
        public ushort[] BindIndices { get; private set; }

        /// <summary>
        /// Gets the names of entries in the <see cref="Skeleton.Bones"/> or <see cref="Model.Materials"/> dictionaries
        /// to bind to for each animation.
        /// </summary>
        public IList<string> Names { get; private set; }

        /// <summary>
        /// Gets <see cref="AnimCurve"/> instances animating properties of objects stored in this section.
        /// </summary>
        public IList<AnimCurve> Curves { get; private set; }
        
        /// <summary>
        /// Gets bytes storing the initial visibility as one bit for each <see cref="Bone"/> or <see cref="Material"/>.
        /// </summary>
        public byte[] BaseDataList { get; private set; } 

        /// <summary>
        /// Gets customly attached <see cref="UserData"/> instances.
        /// </summary>
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

    /// <summary>
    /// Represents flags specifying how animation data is stored or should be played.
    /// </summary>
    [Flags]
    public enum VisibilityAnimFlags : ushort
    {
        /// <summary>
        /// The stored curve data has been baked.
        /// </summary>
        BakedCurve = 1 << 0,

        /// <summary>
        /// The animation repeats from the start after the last frame has been played.
        /// </summary>
        Looping = 1 << 2
    }

    /// <summary>
    /// Represents the kind of data the visibility animation controls.
    /// </summary>
    public enum VisibilityAnimType : ushort
    {
        /// <summary>
        /// Bone visiblity is controlled.
        /// </summary>
        Bone,

        /// <summary>
        /// Material visibility is controlled.
        /// </summary>
        Material = 1 << 8
    }
}