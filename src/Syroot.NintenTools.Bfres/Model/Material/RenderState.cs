using System;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;
using Syroot.NintenTools.Bfres.GX2;

namespace Syroot.NintenTools.Bfres
{
    public class RenderState : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const uint _flagsMaskMode = 0b00000000_00000000_00000000_00000011;
        private const uint _flagsMaskBlendMode = 0b00000000_00000000_00000000_00110000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private uint _flags;
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        public RenderStateFlagsMode FlagsMode
        {
            get { return (RenderStateFlagsMode)(_flags & _flagsMaskMode); }
            set { _flags &= ~_flagsMaskMode | (uint)value; }
        }

        public RenderStateFlagsBlendMode FlagsBlendMode
        {
            get { return (RenderStateFlagsBlendMode)(_flags & _flagsMaskBlendMode); }
            set { _flags &= ~_flagsMaskBlendMode | (uint)value; }
        }

        public PolygonControl PolygonControl { get; set; }

        public DepthControl DepthControl { get; set; }

        public AlphaControl AlphaControl { get; set; }

        public ColorControl ColorControl { get; set; }

        public BlendControl BlendControl { get; set; }

        public Vector4F BlendColor { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            _flags = loader.ReadUInt32();
            PolygonControl = new PolygonControl(loader.ReadUInt32());
            DepthControl = new DepthControl(loader.ReadUInt32());
            AlphaControl = new AlphaControl(loader.ReadUInt32(), loader.ReadSingle());
            ColorControl = new ColorControl(loader.ReadUInt32());
            BlendControl = new BlendControl(loader.ReadUInt32(), loader.ReadUInt32());
            BlendColor = loader.ReadVector4F();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.Write(_flags);
            saver.Write(PolygonControl.Value);
            saver.Write(DepthControl.Value);
            saver.Write(AlphaControl.Value);
            saver.Write(AlphaControl.RefValue);
            saver.Write(ColorControl.Value);
            saver.Write(BlendControl.Target);
            saver.Write(BlendControl.Value);
            saver.Write(BlendColor);
        }
    }
    
    public enum RenderStateFlagsMode : uint
    {
        Custom,
        Opaque,
        AlphaMask,
        Translucent
    }

    public enum RenderStateFlagsBlendMode : uint
    {
        None,
        Color,
        Logical
    }
}