using System;
using System.Collections.Generic;
using System.Text;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FSKL section in a <see cref="Model"/> subfile, storing armature data.
    /// </summary>
    public class Skeleton : ResContent
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const uint _flagsScaleMask = 0b00000000_00000000_00000011_00000000;
        private const uint _flagsRotateMask = 0b00000000_00000000_01110000_00000000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private uint _flags;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public Skeleton(ResFileLoader loader)
            : base(loader)
        {
            SkeletonHead head = new SkeletonHead(loader);
            _flags = head.Flags;
            Bones = loader.LoadDictList<Bone>(head.OfsBoneDict);
            if (head.OfsMatrixToBoneTable != 0)
            {
                loader.Position = head.OfsMatrixToBoneTable;
                MatrixToBoneTable = loader.ReadUInt16s((int)(head.NumSmoothMatrix + head.NumRigidMatrix));
            }
            if (head.OfsInverseModelMatrixList != 0)
            {
                loader.Position = head.OfsInverseModelMatrixList;
                InverseModelMatrices = loader.ReadMatrix3x4s((int)head.NumSmoothMatrix);
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public SkeletonFlagsScale FlagsScale
        {
            get { return (SkeletonFlagsScale)(_flags & _flagsScaleMask); }
            set { _flags &= ~_flagsScaleMask | (uint)value; }
        }

        public SkeletonFlagsRotation FlagsRotation
        {
            get { return (SkeletonFlagsRotation)(_flags & _flagsRotateMask); }
            set { _flags &= ~_flagsRotateMask | (uint)value; }
        }
        
        public IList<Bone> Bones { get; }

        public IList<ushort> MatrixToBoneTable { get; }

        public IList<Matrix3x4> InverseModelMatrices { get; }
    }

    /// <summary>
    /// Represents the header of a <see cref="Skeleton"/> instance.
    /// </summary>
    internal class SkeletonHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSKL";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint Flags;
        internal uint NumBone;
        internal uint NumSmoothMatrix;
        internal uint NumRigidMatrix;
        internal uint OfsBoneDict;
        internal uint OfsBoneList;
        internal uint OfsMatrixToBoneTable;
        internal uint OfsInverseModelMatrixList;
        internal uint UserPointer;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal SkeletonHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            Flags = loader.ReadUInt32();
            NumBone = loader.ReadUInt16();
            NumSmoothMatrix = loader.ReadUInt16();
            NumRigidMatrix = loader.ReadUInt16();
            loader.Seek(2);
            OfsBoneDict = loader.ReadOffset();
            OfsBoneList = loader.ReadOffset();
            OfsMatrixToBoneTable = loader.ReadOffset();
            OfsInverseModelMatrixList = loader.ReadOffset();
            UserPointer = loader.ReadUInt32();
        }
    }

    public enum SkeletonFlagsScale : uint
    {
        None,
        Standard = 1 << 8,
        Maya = 2 << 8,
        Softimage = 3 << 8
    }

    public enum SkeletonFlagsRotation : uint
    {
        Quaternion,
        EulerXYZ = 1 << 12
    }
}
