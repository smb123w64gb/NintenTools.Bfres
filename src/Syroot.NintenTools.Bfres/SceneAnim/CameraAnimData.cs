using System.Runtime.InteropServices;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraAnimData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public float ClipNear;
        public float ClipFar;
        public float AspectRatio;
        public float FieldOfView;
        public Vector3F Position;
        public Vector3F Rotation;
        public float Twist;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public CameraAnimData(ResFileLoader loader)
        {
            ClipNear = loader.ReadSingle();
            ClipFar = loader.ReadSingle();
            AspectRatio = loader.ReadSingle();
            FieldOfView = loader.ReadSingle();
            Position = loader.ReadVector3F();
            Rotation = loader.ReadVector3F();
            Twist = loader.ReadSingle();
        }
    }

    public enum CameraAnimDataOffset : uint
    {
        ClipNear = 0x00,
        ClipFar = 0x04,
        AspectRatio = 0x08,
        FieldOFView = 0x0C,
        PositionX = 0x10,
        PositionY = 0x14,
        PositionZ = 0x18,
        RotationX = 0x1C,
        RotationY = 0x20,
        RotationZ = 0x24,
        Twist = 0x28
    }
}
