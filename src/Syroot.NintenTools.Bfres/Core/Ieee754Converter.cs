using System;

namespace Syroot.NintenTools.Bfres.Core
{
    internal static class Ieee754Converter
    {
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        internal static float ToSingle(byte[] value, int startBitIndex, Ieee754SingleFormat format)
        {
            switch (format)
            {
                case Ieee754SingleFormat.Single10:
                    break;
                case Ieee754SingleFormat.Single11:
                    break;
                case Ieee754SingleFormat.Single14:
                    break;
                case Ieee754SingleFormat.Single16:
                    break;
                case Ieee754SingleFormat.Single32:
                    return BitConverter.ToSingle(value, 0);
                default:
                    throw new NotImplementedException("Unknown IEE754 format.");
            }
        }

        internal static double ToDouble(byte[] value, Ieee754DoubleFormat format)
        {
            switch (format)
            {
                case Ieee754DoubleFormat.Double64:
                    return BitConverter.ToDouble(value, 0);
                default:
                    throw new NotImplementedException("Unknown IEE754 format.");
            }
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static float DecodeSingle(
    }

    internal enum Ieee754SingleFormat
    {
        Single10 = 10,
        Single11 = 11,
        Single14 = 14,
        Single16 = 16,
        Single32 = 32
    }

    internal enum Ieee754DoubleFormat
    {
        Double64 = 64
    }
}
