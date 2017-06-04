using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres.GX2
{
    public class AlphaControl
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _alphaFuncBit = 0, _alphaFuncBits = 3;
        private const int _alphaFuncEnabledBit = 3;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal AlphaControl(uint value, float refValue)
        {
            Value = value;
            RefValue = refValue;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public uint Value { get; set; }

        public bool AlphaTestEnabled
        {
            get { return Value.GetBit(_alphaFuncEnabledBit); }
            set { Value = Value.SetBit(_alphaFuncEnabledBit, value); }
        }

        public GX2CompareFunction AlphaFunc
        {
            get { return (GX2CompareFunction)Value.Decode(_alphaFuncBit, _alphaFuncBits); }
            set { Value = Value.Encode((uint)value, _alphaFuncBit, _alphaFuncBits); }
        }

        public float RefValue { get; set; }
    }
}