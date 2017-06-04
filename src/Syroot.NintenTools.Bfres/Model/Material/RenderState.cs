using System;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;
using Syroot.NintenTools.Bfres.GX2;

namespace Syroot.NintenTools.Bfres
{
    public class RenderState : IResContent
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

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public void Load(ResFileLoader loader)
        {
            RenderStateHead head = new RenderStateHead(loader);
            _flags = head.Flags;
            PolygonControl = new PolygonControl(head.PolygonControl);
            DepthControl = new DepthControl(head.DepthControl);
            AlphaControl = new AlphaControl(head.AlphaControl, head.AlphaControlRefValue);
            ColorControl = new ColorControl(head.ColorControl);
            BlendControl = new BlendControl(head.BlendControl, head.BlendControlTarget);
            BlendColor = head.BlendColor;
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="RenderState"/> instance.
    /// </summary>
    internal class RenderStateHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Flags;
        internal uint PolygonControl;
        internal uint DepthControl;
        internal uint AlphaControl;
        internal float AlphaControlRefValue;
        internal uint ColorControl;
        internal uint BlendControlTarget;
        internal uint BlendControl;
        internal Vector4F BlendColor;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal RenderStateHead(ResFileLoader loader)
        {
            Flags = loader.ReadUInt32();
            PolygonControl = loader.ReadUInt32();
            DepthControl = loader.ReadUInt32();
            AlphaControl = loader.ReadUInt32();
            AlphaControlRefValue = loader.ReadSingle();
            ColorControl = loader.ReadUInt32();
            BlendControlTarget = loader.ReadUInt32();
            BlendControl = loader.ReadUInt32();
            BlendColor = loader.ReadVector4F();
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