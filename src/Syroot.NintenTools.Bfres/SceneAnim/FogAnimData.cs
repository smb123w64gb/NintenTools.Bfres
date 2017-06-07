using System.Runtime.InteropServices;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FogAnimData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public Vector2F DistanceAttn;
        public Vector3F Color;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public FogAnimData(ResFileLoader loader)
        {
            DistanceAttn = loader.ReadVector2F();
            Color = loader.ReadVector3F();
        }
    }

    public enum FogAnimDataOffset : uint
    {
        DistanceAttnX = 0x00,
        DistanceAttnY = 0x04,
        ColorR = 0x08,
        ColorG = 0x0C,
        ColorB = 0x10
    }
}
