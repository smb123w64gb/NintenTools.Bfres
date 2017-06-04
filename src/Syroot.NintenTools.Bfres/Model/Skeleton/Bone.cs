using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a single bone in a <see cref="Skeleton"/> section, storing its initial transform and transformation
    /// effects.
    /// </summary>
    [DebuggerDisplay(nameof(Bone) + " {" + nameof(Name) + "}")]
    public class Bone : ResContent
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const uint _flagsMask = 0b00000000_00000000_00000000_00000001;
        private const uint _flagsMaskScale = 0b00000000_00000000_00000011_00000000;
        private const uint _flagsMaskRotation = 0b00000000_00000000_01110000_00000000;
        private const uint _flagsMaskBillboard = 0b00000000_00000111_00000000_00000000;
        private const uint _flagsMaskTransform = 0b00001111_10000000_00000000_00000000;
        private const uint _flagsMaskTransformCumulative = 0b11110000_00000000_00000000_00000000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private uint _flags;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public Bone(ResFileLoader loader)
            : base(loader)
        {
            BoneHead head = new BoneHead(loader);
            Name = loader.GetName(head.OfsName);
            ParentIndex = head.IdxParent;
            SmoothMatrixIndex = head.IdxSmoothMatrix;
            RigidMatrixIndex = head.IdxRigidMatrix;
            BillboardIndex = head.IdxRigidMatrix;
            _flags = head.Flags;
            Scale = head.Scale;
            Rotation = head.Rotation;
            Position = head.Position;
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string Name { get; set; }

        public ushort ParentIndex { get; set; }

        public short SmoothMatrixIndex { get; set; }

        public short RigidMatrixIndex { get; set; }

        public short BillboardIndex { get; set; }

        public BoneFlags Flags
        {
            get { return (BoneFlags)(_flags & _flagsMask); }
            set { _flags &= ~_flagsMask | (uint)value; }
        }

        public BoneFlagsRotation FlagsRotation
        {
            get { return (BoneFlagsRotation)(_flags & _flagsMaskRotation); }
            set { _flags &= ~_flagsMaskRotation | (uint)value; }
        }

        public BoneFlagsBillboard FlagsBillboard
        {
            get { return (BoneFlagsBillboard)(_flags & _flagsMaskBillboard); }
            set { _flags &= ~_flagsMaskBillboard | (uint)value; }
        }

        public BoneFlagsTransform FlagsTransform
        {
            get { return (BoneFlagsTransform)(_flags & _flagsMaskTransform); }
            set { _flags &= ~_flagsMaskTransform | (uint)value; }
        }

        public BoneFlagsTransformCumulative FlagsTransformCumulative
        {
            get { return (BoneFlagsTransformCumulative)(_flags & _flagsMaskTransformCumulative); }
            set { _flags &= ~_flagsMaskTransformCumulative | (uint)value; }
        }

        public Vector3F Scale { get; set; }

        public Vector4F Rotation { get; set; }

        public Vector3F Position { get; set; }

        public IList<UserData> UserData { get; }
    }
    
    /// <summary>
    /// Represents the header of a <see cref="Bone"/> instance.
    /// </summary>
    internal class BoneHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint OfsName;
        internal ushort Idx;
        internal ushort IdxParent;
        internal short IdxSmoothMatrix;
        internal short IdxRigidMatrix;
        internal ushort IdxBillboard;
        internal ushort NumUserData;
        internal uint Flags;
        internal Vector3F Scale;
        internal Vector4F Rotation;
        internal Vector3F Position;
        internal uint OfsUserDataDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal BoneHead(ResFileLoader loader)
        {
            OfsName = loader.ReadOffset();
            Idx = loader.ReadUInt16();
            IdxParent = loader.ReadUInt16();
            IdxSmoothMatrix = loader.ReadInt16();
            IdxRigidMatrix = loader.ReadInt16();
            IdxBillboard = loader.ReadUInt16();
            NumUserData = loader.ReadUInt16();
            Flags = loader.ReadUInt32();
            Scale = loader.ReadVector3F();
            Rotation = loader.ReadVector4F();
            Position = loader.ReadVector3F();
            OfsUserDataDict = loader.ReadOffset();
        }
    }
    
    public enum BoneFlags : uint
    {
        Visible = 1 << 0   
    }
    
    public enum BoneFlagsRotation : uint
    {
        Quaternion,
        EulerXYZ = 1 << 12
    }
    
    public enum BoneFlagsBillboard : uint
    {
        None,
        Child = 1 <<16,
        WorldViewVector = 2 << 16,
        WorldViewPoint = 3 << 16,
        ScreenViewVector = 4 << 16,
        ScreenViewPoint = 5 << 16,
        YAxisViewVector = 6 << 16,
        YAxisViewPoint = 7 << 16
    }

    [Flags]
    public enum BoneFlagsTransform : uint
    {
        SegmentScaleCompensate = 1 << 23,
        ScaleUniform = 1 << 24,
        ScaleVolumeOne = 1 << 25,
        RotateZero = 1 << 26,
        TranslateZero = 1 << 27,
        ScaleOne =  ScaleVolumeOne | ScaleUniform,
        RotateTranslateZero = RotateZero | TranslateZero,
        Identity = ScaleOne | RotateZero | TranslateZero
    }

    [Flags]
    public enum BoneFlagsTransformCumulative : uint
    {
        None,
        ScaleUniform = 1 << 28,
        ScaleVolumeOne = 1 << 29,
        RotateZero = 1 << 30,
        TranslateZero = 1u << 31,
        ScaleOne = ScaleVolumeOne | ScaleUniform,
        RotateTranslateZero = RotateZero | TranslateZero,
        Identity = ScaleOne | RotateZero | TranslateZero
    }
}