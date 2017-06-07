using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents the animation of a single <see cref="Bone"/> in a <see cref="SkeletalAnim"/> subfile.
    /// </summary>
    [DebuggerDisplay(nameof(BoneAnim) + " {" + nameof(Name) + "}")]
    public class BoneAnim : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const uint _flagsMaskBase = 0b00000000_00000000_00000000_00111000;
        private const uint _flagsMaskCurve = 0b00000000_00000000_11111111_11000000;
        private const uint _flagsMaskTransform = 0b00001111_10000000_00000000_00000000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private uint _flags;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public BoneAnimFlagsBase FlagsBase
        {
            get { return (BoneAnimFlagsBase)(_flags & _flagsMaskBase); }
            set { _flags &= ~_flagsMaskBase | (uint)value; }
        }

        public BoneAnimFlagsCurve FlagsCurve
        {
            get { return (BoneAnimFlagsCurve)(_flags & _flagsMaskCurve); }
            set { _flags &= ~_flagsMaskCurve | (uint)value; }
        }

        public BoneAnimFlagsTransform FlagsTransform
        {
            get { return (BoneAnimFlagsTransform)(_flags & _flagsMaskTransform); }
            set { _flags &= ~_flagsMaskTransform | (uint)value; }
        }

        public string Name { get; set; }

        public byte BeginRotate { get; set; }

        public byte BeginTranslate { get; set; }

        public byte BeginBaseTranslate { get; set; }

        public int BeginCurve { get; set; }

        public IList<AnimCurve> Curves { get; private set; }

        public BoneAnimData BaseData { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            BoneAnimHead head = new BoneAnimHead(loader);
            using (loader.TemporarySeek())
            {
                _flags = head.Flags;
                Name = loader.GetName(head.OfsName);
                BeginRotate = head.BeginRotate;
                BeginTranslate = head.BeginTranslate;
                BeginBaseTranslate = head.BeginBaseTranslate;
                BeginCurve = head.BeginCurve;
                Curves = loader.LoadList<AnimCurve>(head.OfsCurveList, head.NumCurve);

                if (head.OfsBaseData != 0)
                {
                    loader.Position = head.OfsBaseData;
                    BaseData = new BoneAnimData(loader, FlagsBase);
                }
            }
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="BoneAnim"/> instance.
    /// </summary>
    internal class BoneAnimHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Flags;
        internal uint OfsName;
        internal byte BeginRotate;
        internal byte BeginTranslate;
        internal byte NumCurve;
        internal byte BeginBaseTranslate;
        internal int BeginCurve;
        internal uint OfsCurveList;
        internal uint OfsBaseData;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal BoneAnimHead(ResFileLoader loader)
        {
            Flags = loader.ReadUInt32();
            OfsName = loader.ReadOffset();
            BeginRotate = loader.ReadByte();
            BeginTranslate = loader.ReadByte();
            NumCurve = loader.ReadByte();
            BeginBaseTranslate = loader.ReadByte();
            BeginCurve = loader.ReadInt32();
            OfsCurveList = loader.ReadOffset();
            OfsBaseData = loader.ReadOffset();
        }
    }

    [Flags]
    public enum BoneAnimFlagsBase : uint
    {
        Scale = 1 << 3,
        Rotate = 1 << 4,
        Translate = 1 << 5
    }

    [Flags]
    public enum BoneAnimFlagsCurve : uint
    {
        ScaleX = 1 << 6,
        ScaleY = 1 << 7,
        ScaleZ = 1 << 8,
        RotateX = 1 << 9,
        RotateY = 1 << 10,
        RotateZ = 1 << 11,
        RotateW = 1 << 12,
        TranslateX = 1 << 13,
        TranslateY = 1 << 14,
        TranslateZ = 1 << 15
    }

    [Flags]
    public enum BoneAnimFlagsTransform : uint // Same as BoneFlagsTransform
    {
        SegmentScaleCompensate = 1 << 23,
        ScaleUniform = 1 << 24,
        ScaleVolumeOne = 1 << 25,
        RotateZero = 1 << 26,
        TranslateZero = 1 << 27,
        ScaleOne = ScaleVolumeOne | ScaleUniform,
        RotateTranslateZero = RotateZero | TranslateZero,
        Identity = ScaleOne | RotateZero | TranslateZero
    }
}