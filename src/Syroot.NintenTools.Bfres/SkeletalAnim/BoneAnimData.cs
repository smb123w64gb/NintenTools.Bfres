using System.Runtime.InteropServices;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BoneAnimData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public uint Flags;
        public Vector3F Scale;
        public Vector3F Translate;
        // public uint Padding;
        public Vector4F Rotate;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal BoneAnimData(ResFileLoader loader, BoneAnimFlagsBase flags)
        {
            Flags = 0; // Never in files.
            Scale = flags.HasFlag(BoneAnimFlagsBase.Scale) ? loader.ReadVector3F() : Vector3F.Zero;
            Translate = flags.HasFlag(BoneAnimFlagsBase.Translate) ? loader.ReadVector3F() : Vector3F.Zero;
            // Padding = 0; // Never in files.
            Rotate = flags.HasFlag(BoneAnimFlagsBase.Rotate) ? loader.ReadVector4F() : Vector4F.Zero;
        }
    }

    public enum BoneAnimDataOffset : uint
    {
        Flags = 0x00,
        ScaleX = 0x04,
        ScaleY = 0x08,
        ScaleZ = 0x0C,
        TranslateX = 0x10,
        TranslateY = 0x14,
        TranslateZ = 0x18,
        // Padding = 0x1C,
        RotateX = 0x20,
        RotateY = 0x24,
        RotateZ = 0x28,
        RotateW = 0x2C
    }
}