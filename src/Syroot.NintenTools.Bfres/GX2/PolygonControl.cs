using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres.GX2
{
    /// <summary>
    /// Represents GX2 polygon drawing settings controlling if and how triangles are rendered.
    /// </summary>
    public class PolygonControl
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _cullFrontBit = 0;
        private const int _cullBackBit = 1;
        private const int _frontFaceBit = 2;
        private const int _polygonModeBit = 3;
        private const int _polygonModeFrontBit = 5, _polygonModeFrontBits = 3;
        private const int _polygonModeBackBit = 8, _polygonModeBackBits = 3;
        private const int _polygonOffsetFrontBit = 11;
        private const int _polygonOffsetBackBit = 12;
        private const int _polygonLineOffsetBit = 13;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonControl"/> class.
        /// </summary>
        public PolygonControl()
        {
        }

        internal PolygonControl(uint value)
        {
            Value = value;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        public bool CullFront
        {
            get { return Value.GetBit(_cullFrontBit); }
            set { Value = Value.SetBit(_cullFrontBit, value); }
        }

        public bool CullBack
        {
            get { return Value.GetBit(_cullBackBit); }
            set { Value = Value.SetBit(_cullBackBit, value); }
        }

        public GX2FrontFaceMode FrontFace
        {
            get { return Value.GetBit(_frontFaceBit) ? GX2FrontFaceMode.Clockwise : GX2FrontFaceMode.CounterClockwise; }
            set { Value = Value.SetBit(_frontFaceBit, value == GX2FrontFaceMode.Clockwise); }
        }

        public bool PolygonModeEnabled
        {
            get { return Value.GetBit(_polygonModeBit); }
            set { Value = Value.SetBit(_polygonModeBit, value); }
        }

        public GX2PolygonMode PolygonModeFront
        {
            get { return (GX2PolygonMode)Value.Decode(_polygonModeFrontBits, _polygonModeFrontBit); }
            set { Value = Value.Encode((uint)value, _polygonModeFrontBits, _polygonModeFrontBit); }
        }

        public GX2PolygonMode PolygonModeBack
        {
            get { return (GX2PolygonMode)Value.Decode(_polygonModeBackBits, _polygonModeBackBit); }
            set { Value = Value.Encode((uint)value, _polygonModeBackBits, _polygonModeBackBit); }
        }

        public bool PolygonOffsetFrontEnabled
        {
            get { return Value.GetBit(_polygonOffsetFrontBit); }
            set { Value = Value.SetBit(_polygonOffsetFrontBit, value); }
        }

        public bool PolygonOffsetBackEnabled
        {
            get { return Value.GetBit(_polygonOffsetBackBit); }
            set { Value = Value.SetBit(_polygonOffsetBackBit, value); }
        }

        public bool PolygonLineOffsetEnabled
        {
            get { return Value.GetBit(_polygonLineOffsetBit); }
            set { Value = Value.SetBit(_polygonLineOffsetBit, value); }
        }

        internal uint Value { get; set; }
    }
}
