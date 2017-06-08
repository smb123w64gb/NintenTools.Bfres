using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FSKA subfile in a <see cref="ResFile"/>, storing armature animations of <see cref="Bone"/>
    /// instances in a <see cref="Skeleton"/>.
    /// </summary>
    [DebuggerDisplay(nameof(SkeletalAnim) + " {" + nameof(Name) + "}")]
    public class SkeletalAnim : INamedResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const uint _flagsMaskScale = 0b00000000_00000000_00000011_00000000;
        private const uint _flagsMaskRotate = 0b00000000_00000000_01110000_00000000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;
        private uint _flags;
        private uint _ofsBindSkeleton;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// The name with which the instance can be referenced uniquely in <see cref="INamedResDataList{SkeletalAnim}"/>
        /// instances.
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

        public SkeletalAnimFlagsScale FlagsScale
        {
            get { return (SkeletalAnimFlagsScale)(_flags & _flagsMaskScale); }
            set { _flags &= ~_flagsMaskScale | (uint)value; }
        }

        public SkeletalAnimFlagsRotate FlagsRotate
        {
            get { return (SkeletalAnimFlagsRotate)(_flags & _flagsMaskRotate); }
            set { _flags &= ~_flagsMaskRotate | (uint)value; }
        }

        public int FrameCount { get; set; }

        public uint BakedSize { get; set; }
        
        public IList<BoneAnim> BoneAnims { get; private set; }

        public Skeleton BindSkeleton { get; set; }

        public ushort[] BindIndices { get; private set; }

        public INamedResDataList<UserData> UserData { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            SkeletalAnimHead head = new SkeletalAnimHead(loader);
            Name = loader.GetName(head.OfsName);
            Path = loader.GetName(head.OfsPath);
            _flags = head.Flags;
            FrameCount = head.NumFrame;
            BakedSize = head.SizBaked;
            BoneAnims = loader.LoadList<BoneAnim>(head.OfsBoneAnimList, head.NumBoneAnim);
            _ofsBindSkeleton = head.OfsBindSkeleton;

            if (head.OfsBindIndexList != 0)
            {
                loader.Position = head.OfsBindIndexList;
                BindIndices = loader.ReadUInt16s(head.NumBoneAnim);
            }

            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        void IResData.Reference(ResFileLoader loader)
        {
            BindSkeleton = loader.GetData<Skeleton>(_ofsBindSkeleton);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="SkeletalAnim"/> instance.
    /// </summary>
    internal class SkeletalAnimHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSKA";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint OfsName;
        internal uint OfsPath;
        internal uint Flags;
        internal int NumFrame;
        internal ushort NumBoneAnim;
        internal ushort NumUserData;
        internal int NumCurve;
        internal uint SizBaked;
        internal uint OfsBoneAnimList;
        internal uint OfsBindSkeleton;
        internal uint OfsBindIndexList;
        internal uint OfsUserDataDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal SkeletalAnimHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            OfsName = loader.ReadOffset();
            OfsPath = loader.ReadOffset();
            Flags = loader.ReadUInt32();
            NumFrame = loader.ReadInt32();
            NumBoneAnim = loader.ReadUInt16();
            NumUserData = loader.ReadUInt16();
            NumCurve = loader.ReadInt32();
            SizBaked = loader.ReadUInt32();
            OfsBoneAnimList = loader.ReadOffset();
            OfsBindSkeleton = loader.ReadOffset();
            OfsBindIndexList = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
        }
    }

    [Flags]
    public enum SkeletalAnimFlags : uint
    {
        BakedCurve = 1 << 0,
        Looping = 1 << 2
    }

    public enum SkeletalAnimFlagsScale : uint
    {
        None,
        Standard = 1 << 8,
        Maya = 2 << 8,
        Softimage = 3 << 8
    }

    public enum SkeletalAnimFlagsRotate : uint
    {
        Quaternion,
        EulerXYZ = 1 << 12
    }
}