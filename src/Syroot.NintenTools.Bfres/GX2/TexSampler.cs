using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres.GX2
{
    public class TexSampler
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _clampXBit = 0, _clampXBits = 3;
        private const int _clampYBit = 3, _clampYBits = 3;
        private const int _clampZBit = 6, _clampZBits = 3;
        private const int _xyMagFilterBit = 9, _xyMagFilterBits = 2;
        private const int _xyMinFilterBit = 12, _xyMinFilterBits = 2;
        private const int _zFilterBit = 15, _zFilterBits = 2;
        private const int _mipFilterBit = 17, _mipFilterBits = 2;
        private const int _maxAnisotropicRatioBit = 19, _maxAnisotropicRatioBits = 3;
        private const int _borderTypeBit = 22, _borderTypeBits = 2;
        private const int _depthCompareFuncBit = 26, _depthCompareFuncBits = 3;

        private const int _minLodBit = 0, _minLodBits = 10;
        private const int _maxLodBit = 0, _maxLodBits = 10;
        private const int _lodBiasBit = 20, _lodBiasBits = 20;

        private const int _depthCompareBit = 30;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public TexSampler(uint[] values)
        {
            Values = values;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public uint[] Values { get; set; }

        public GX2TexClamp ClampX
        {
            get { return (GX2TexClamp)Values[0].Decode(_clampXBit, _clampXBits); }
            set { Values[0] = Values[0].Encode((uint)value, _clampXBit, _clampXBits); }
        }

        public GX2TexClamp ClampY
        {
            get { return (GX2TexClamp)Values[0].Decode(_clampYBit, _clampYBits); }
            set { Values[0] = Values[0].Encode((uint)value, _clampYBit, _clampYBits); }
        }

        public GX2TexClamp ClampZ
        {
            get { return (GX2TexClamp)Values[0].Decode(_clampZBit, _clampZBits); }
            set { Values[0] = Values[0].Encode((uint)value, _clampZBit, _clampZBits); }
        }

        public GX2TexXYFilterType MagFilter
        {
            get { return (GX2TexXYFilterType)Values[0].Decode(_xyMagFilterBit, _xyMagFilterBits); }
            set { Values[0] = Values[0].Encode((uint)value, _xyMagFilterBit, _xyMagFilterBits); }
        }

        public GX2TexXYFilterType MinFilter
        {
            get { return (GX2TexXYFilterType)Values[0].Decode(_xyMinFilterBit, _xyMinFilterBits); }
            set { Values[0] = Values[0].Encode((uint)value, _xyMinFilterBit, _xyMinFilterBits); }
        }

        public GX2TexZFilterType ZFilter
        {
            get { return (GX2TexZFilterType)Values[0].Decode(_zFilterBit, _zFilterBits); }
            set { Values[0] = Values[0].Encode((uint)value, _zFilterBit, _zFilterBits); }
        }

        public GX2TexMipFilterType MipFilter
        {
            get { return (GX2TexMipFilterType)Values[0].Decode(_mipFilterBit, _mipFilterBits); }
            set { Values[0] = Values[0].Encode((uint)value, _mipFilterBit, _mipFilterBits); }
        }

        public GX2TexAnisoRatio MaxAnisotropicRatio
        {
            get { return (GX2TexAnisoRatio)Values[0].Decode(_maxAnisotropicRatioBit, _maxAnisotropicRatioBits); }
            set { Values[0] = Values[0].Encode((uint)value, _maxAnisotropicRatioBit, _maxAnisotropicRatioBits); }
        }

        public GX2TexBorderType BorderType
        {
            get { return (GX2TexBorderType)Values[0].Decode(_borderTypeBit, _borderTypeBits); }
            set { Values[0] = Values[0].Encode((uint)value, _borderTypeBit, _borderTypeBits); }
        }

        public GX2CompareFunction DepthCompareFunc
        {
            get { return (GX2CompareFunction)Values[0].Decode(_depthCompareFuncBit, _depthCompareFuncBits); }
            set { Values[0] = Values[0].Encode((uint)value, _depthCompareFuncBit, _depthCompareFuncBits); }
        }
        
        public float MinLod
        {
            get { return UInt32ToSingle(Values[1].Decode(_minLodBit, _minLodBits)); }
            set { Values[1].Encode(SingleToUInt32(value), _minLodBit, _minLodBits); }
        }
        
        public float MaxLod
        {
            get { return UInt32ToSingle(Values[1].Decode(_maxLodBit, _maxLodBits)); }
            set { Values[1].Encode(SingleToUInt32(value), _maxLodBit, _maxLodBits); }
        }
        
        public float LodBias
        {
            get { return UInt32ToSingle(Values[1].Decode(_lodBiasBit, _lodBiasBits)); }
            set { Values[1].Encode(SingleToUInt32(value), _lodBiasBit, _lodBiasBits); }
        }

        public bool DepthCompareEnabled
        {
            get { return Values[2].GetBit(_depthCompareBit); }
            set { Values[2] = Values[2].SetBit(_depthCompareBit, value); }
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private float UInt32ToSingle(uint value)
        {
            return value / 64f; // TODO: Validate correctness of conversion.
        }

        private uint SingleToUInt32(float value)
        {
            return (uint)(Algebra.Clamp(value, 0, 13) * 64f); // TODO: Validate correctness of conversion.
        }
    }
}
