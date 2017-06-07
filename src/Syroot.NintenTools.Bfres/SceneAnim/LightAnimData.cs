using System.Runtime.InteropServices;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LightAnimData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public int Enable;
        public Vector3F Position;
        public Vector3F Rotation;
        public Vector2F DistanceAttn;
        public Vector2F AngleAttn;
        public Vector3F Color0;
        public Vector3F Color1;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal LightAnimData(ResFileLoader loader, LightAnimFlags flags)
        {
            Enable = flags.HasFlag(LightAnimFlags.ResultEnable) ? loader.ReadInt32() : 0;
            Position = flags.HasFlag(LightAnimFlags.ResultPosition) ? loader.ReadVector3F() : Vector3F.Zero;
            Rotation = flags.HasFlag(LightAnimFlags.ResultRotation) ? loader.ReadVector3F() : Vector3F.Zero;
            DistanceAttn = flags.HasFlag(LightAnimFlags.ResultDistanceAttn) ? loader.ReadVector2F() : Vector2F.Zero;
            AngleAttn = flags.HasFlag(LightAnimFlags.ResultAngleAttn) ? loader.ReadVector2F() : Vector2F.Zero;
            Color0 = flags.HasFlag(LightAnimFlags.ResultColor0) ? loader.ReadVector3F() : Vector3F.Zero;
            Color1 = flags.HasFlag(LightAnimFlags.ResultColor1) ? loader.ReadVector3F() : Vector3F.Zero;
        }
    }

    public enum LightAnimDataOffset : uint
    {
        Enable = 0x00,
        PositionX = 0x04,
        PositionY = 0x08,
        PositionZ = 0x0C,
        RotationX = 0x10,
        RotationY = 0x14,
        RotationZ = 0x18,
        DistanceAttnX = 0x1C,
        DistanceAttnY = 0x20,
        AngleAttnX = 0x24,
        AngleAttnY = 0x28,
        Color0R = 0x2C,
        Color0G = 0x30,
        Color0B = 0x34,
        Color1R = 0x38,
        Color1G = 0x3C,
        Color1B = 0x40
    }
}
