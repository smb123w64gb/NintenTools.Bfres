using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres.GX2
{
    /// <summary>
    /// Represents GX2 settings controlling how depth and stencil buffer checks are performed and handled.
    /// </summary>
    public class DepthControl
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _stencilBit = 0;
        private const int _depthTestBit = 1;
        private const int _depthWriteBit = 2;
        private const int _depthFuncBit = 4, _depthFuncBits = 3;
        private const int _backStencilBit = 7;
        private const int _frontStencilFuncBit = 8, _frontStencilFuncBits = 3;
        private const int _frontStencilFailBit = 11, _frontStencilFailBits = 3;
        private const int _frontStencilZPassBit = 14, _frontStencilZPassBits = 3;
        private const int _frontStencilZFailBit = 17, _frontStencilZFailBits = 3;
        private const int _backStencilFuncBit = 20, _backStencilFuncBits = 3;
        private const int _backStencilFailBit = 23, _backStencilFailBits = 3;
        private const int _backStencilZPassBit = 26, _backStencilZPassBits = 3;
        private const int _backStencilZFailBit = 29, _backStencilZFailBits = 3;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthControl"/> class.
        /// </summary>
        public DepthControl()
        {
        }

        internal DepthControl(uint value)
        {
            Value = value;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        public bool DepthTestEnabled
        {
            get { return Value.GetBit(_depthTestBit); }
            set { Value = Value.SetBit(_depthTestBit, value); }
        }

        public bool DepthWriteEnabled
        {
            get { return Value.GetBit(_depthWriteBit); }
            set { Value = Value.SetBit(_depthWriteBit, value); }
        }

        public GX2CompareFunction DepthFunc
        {
            get { return (GX2CompareFunction)Value.Decode(_depthFuncBit, _depthFuncBits); }
            set { Value = Value.Encode((uint)value, _depthFuncBit, _depthFuncBits); }
        }

        public bool StencilTestEnabled
        {
            get { return Value.GetBit(_stencilBit); }
            set { Value = Value.SetBit(_stencilBit, value); }
        }

        public bool BackStencilEnabled
        {
            get { return Value.GetBit(_backStencilBit); }
            set { Value = Value.SetBit(_backStencilBit, value); }
        }

        public GX2CompareFunction FrontStencilFunc
        {
            get { return (GX2CompareFunction)Value.Decode(_frontStencilFuncBit, _frontStencilFuncBits); }
            set { Value = Value.Encode((uint)value, _frontStencilFuncBit, _frontStencilFuncBits); }
        }

        public GX2StencilFunction FrontStencilFail
        {
            get { return (GX2StencilFunction)Value.Decode(_frontStencilFailBit, _frontStencilFailBits); }
            set { Value = Value.Encode((uint)value, _frontStencilFailBit, _frontStencilFailBits); }
        }

        public GX2StencilFunction FrontStencilZPass
        {
            get { return (GX2StencilFunction)Value.Decode(_frontStencilZPassBit, _frontStencilZPassBits); }
            set { Value = Value.Encode((uint)value, _frontStencilZPassBit, _frontStencilZPassBits); }
        }

        public GX2StencilFunction FrontStencilZFail
        {
            get { return (GX2StencilFunction)Value.Decode(_frontStencilZFailBit, _frontStencilZFailBits); }
            set { Value = Value.Encode((uint)value, _frontStencilZFailBit, _frontStencilZFailBits); }
        }

        public GX2CompareFunction BackStencilFunc
        {
            get { return (GX2CompareFunction)Value.Decode(_backStencilFuncBit, _backStencilFuncBits); }
            set { Value = Value.Encode((uint)value, _backStencilFuncBit, _backStencilFuncBits); }
        }

        public GX2StencilFunction BackStencilFail
        {
            get { return (GX2StencilFunction)Value.Decode(_backStencilFailBit, _backStencilFailBits); }
            set { Value = Value.Encode((uint)value, _backStencilFailBit, _backStencilFailBits); }
        }

        public GX2StencilFunction BackStencilZPass
        {
            get { return (GX2StencilFunction)Value.Decode(_backStencilZPassBit, _backStencilZPassBits); }
            set { Value = Value.Encode((uint)value, _backStencilZPassBit, _backStencilZPassBits); }
        }

        public GX2StencilFunction BackStencilZFail
        {
            get { return (GX2StencilFunction)Value.Decode(_backStencilZFailBit, _backStencilZFailBits); }
            set { Value = Value.Encode((uint)value, _backStencilZFailBit, _backStencilZFailBits); }
        }

        internal uint Value { get; set; }
    }
}