namespace Syroot.NintenTools.Bfres.GX2
{
    /// <summary>
    /// Represents the format of a vertex attribute entry.
    /// Possible type conversions:
    /// - UNorm: attrib unsigned integer is converted to/from [0.0, 1.0] in shader
    /// - UInt: attrib unsigned integer is copied to/from shader as unsigned int
    /// - SNorm: attrib signed integer is converted to/from [-1.0, 1.0] in shader
    /// - SInt: attrib signed integer is copied to/from shader as signed int
    /// - Single: attrib single is copied to/from shader as Single
    /// - UIntToSingle: attrib unsigned integer is converted Single in shader
    /// - SIntToSingle: attrib signed integer is converted Single in shader
    /// (32 bit integers cannot be converted to Single during fetch)
    /// </summary>
    public enum GX2AttribFormat
    {
        // 8 bits (8x1)
        Format_8_UNorm = 0x00000000,
        Format_8_UInt = 0x00000100,
        Format_8_SNorm = 0x00000200,
        Format_8_SInt = 0x00000300,
        Format_8_UIntToSingle = 0x00000800,
        Format_8_SIntToSingle = 0x00000a00,
        // 8 bits (4x2)
        Format_4_4_UNorm = 0x00000001,
        // 16 bits (16x1)
        Format_16_UNorm = 0x00000002,
        Format_16_UInt = 0x00000102,
        Format_16_SNorm = 0x00000202,
        Format_16_SInt = 0x00000302,
        Format_16_Single = 0x00000803,
        Format_16_UIntToSingle = 0x00000802,
        Format_16_SIntToSingle = 0x00000a02,
        // 16 bits (8x2)
        Format_8_8_UNorm = 0x00000004,
        Format_8_8_UInt = 0x00000104,
        Format_8_8_SNorm = 0x00000204,
        Format_8_8_SInt = 0x00000304,
        Format_8_8_UIntToSingle = 0x00000804,
        Format_8_8_SIntToSingle = 0x00000a04,
        // 32 bits (32x1)
        Format_32_UInt = 0x00000105,
        Format_32_SInt = 0x00000305,
        Format_32_Single = 0x00000806,
        // 32 bits (16x2)
        Format_16_16_UNorm = 0x00000007,
        Format_16_16_UInt = 0x00000107,
        Format_16_16_SNorm = 0x00000207,
        Format_16_16_SInt = 0x00000307,
        Format_16_16_Single = 0x00000808,
        Format_16_16_UIntToSingle = 0x00000807,
        Format_16_16_SIntToSingle = 0x00000a07,
        // 32 bits (10/11x3)
        Format_10_11_11_Single = 0x00000809,
        // 32 bits (8x4)
        Format_8_8_8_8_UNorm = 0x0000000a,
        Format_8_8_8_8_UInt = 0x0000010a,
        Format_8_8_8_8_SNorm = 0x0000020a,
        Format_8_8_8_8_SInt = 0x0000030a,
        Format_8_8_8_8_UIntToSingle = 0x0000080a,
        Format_8_8_8_8_SIntToSingle = 0x00000a0a,
        // 32 bits (10x3 + 2)
        Format_10_10_10_2_UNorm = 0x0000000b,
        Format_10_10_10_2_UInt = 0x0000010b,
        Format_10_10_10_2_SNorm = 0x0000020b, // Last 2 bits are UNorm
        Format_10_10_10_2_SInt = 0x0000030b,
        // 64 bits (32x2)
        Format_32_32_UInt = 0x0000010c,
        Format_32_32_SInt = 0x0000030c,
        Format_32_32_Single = 0x0000080d,
        // 64 bits (16x4)
        Format_16_16_16_16_UNorm = 0x0000000e,
        Format_16_16_16_16_UInt = 0x0000010e,
        Format_16_16_16_16_SNorm = 0x0000020e,
        Format_16_16_16_16_SInt = 0x0000030e,
        Format_16_16_16_16_Single = 0x0000080f,
        Format_16_16_16_16_UIntToSingle = 0x0000080e,
        Format_16_16_16_16_SIntToSingle = 0x00000a0e,
        // 96 bits (32x3)
        Format_32_32_32_UInt = 0x00000110,
        Format_32_32_32_SInt = 0x00000310,
        Format_32_32_32_Single = 0x00000811,
        // 128 bits (32x4)
        Format_32_32_32_32_UInt = 0x00000112,
        Format_32_32_32_32_SInt = 0x00000312,
        Format_32_32_32_32_Single = 0x00000813
    }

    /// <summary>
    /// Represents how the terms of the blend function are combined.
    /// </summary>
    public enum GX2BlendCombine
    {
        Add,
        SourceMinusDestination,
        Minimum,
        Maximum,
        DestinationMinusSource
    }

    /// <summary>
    /// Represents the factors used in the blend function.
    /// </summary>
    public enum GX2BlendFunction
    {
        Zero = 0,
        One = 1,
        SourceColor = 2,
        OneMinusSourceColor = 3,
        SourceAlpha = 4,
        OneMinusSourceAlpha = 5,
        DestinationAlpha = 6,
        OneMinusDestinationAlpha = 7,
        DestinationColor = 8,
        OneMinusDestinationColor = 9,
        SourceAlphaSaturate = 10,
        ConstantColor = 13,
        OneMinusConstantColor = 14,
        Source1Color = 15,
        OneMinusSource1Color = 16,
        Source1Alpha = 17,
        OneMinusSource1Alpha = 18,
        ConstantAlpha = 19,
        OneMinusConstantAlpha = 20,
    }

    /// <summary>
    /// Represents compare functions used for depth and stencil tests.
    /// </summary>
    public enum GX2CompareFunction
    {
        Never,
        Less,
        Equal,
        LessOrEqual,
        Greater,
        NotEqual,
        GreaterOrEqual,
        Always
    }

    /// <summary>
    /// Represents the vertex order of front-facing polygons.
    /// </summary>
    public enum GX2FrontFaceMode
    {
        CounterClockwise,
        Clockwise
    }

    /// <summary>
    /// Represents the type in which vertex indices are stored.
    /// </summary>
    public enum GX2IndexFormat
    {
        UInt16LittleEndian = 0,
        UInt32LittleEndian = 1,
        UInt16 = 4,
        UInt32 = 9,
    }

    /// <summary>
    /// Represents the logic op function to perform.
    /// </summary>
    public enum GX2LogicOp
    {
        /// <summary>
        /// Black
        /// </summary>
        Clear = 0x00,
        /// <summary>
        /// White
        /// </summary>
        Set = 0xFF,
        /// <summary>
        /// Source (Default)
        /// </summary>
        Copy = 0xCC,
        /// <summary>
        /// ~Source
        /// </summary>
        InverseCopy = 0x33,
        /// <summary>
        /// Destination
        /// </summary>
        NoOperation = 0xAA,
        /// <summary>
        /// ~Destination
        /// </summary>
        Inverse = 0x55,
        /// <summary>
        /// Source & Destination
        /// </summary>
        And = 0x88,
        /// <summary>
        /// ~(Source & Destination)
        /// </summary>
        NAnd = 0x77,
        /// <summary>
        /// Source | Destination
        /// </summary>
        Or = 0xEE,
        /// <summary>
        /// ~(Source | Destination)
        /// </summary>
        NOr = 0x11,
        /// <summary>
        /// Source ^ Destination
        /// </summary>
        XOr = 0x66,
        /// <summary>
        ///  ~(Source ^ Destination)
        /// </summary>
        Equivalent = 0x99,
        /// <summary>
        /// Source & ~Destination
        /// </summary>
        ReverseAnd = 0x44,
        /// <summary>
        /// ~Source & Destination
        /// </summary>
        InverseAnd = 0x22,
        /// <summary>
        /// Source | ~Destination
        /// </summary>
        ReverseOr = 0xDD,
        /// <summary>
        /// ~Source | Destination
        /// </summary>
        InverseOr = 0xBB,
    }

    /// <summary>
    /// Represents the base primitive used to draw each side of the polygon when dual-sided polygon mode is enabled.
    /// </summary>
    public enum GX2PolygonMode
    {
        Point,
        Line,
        Triangle
    }

    /// <summary>
    /// Represents the type of primitives to draw.
    /// </summary>
    public enum GX2PrimitiveType
    {
        /// <summary>
        /// Requires at least 1 element and 1 more to draw another primitive.
        /// </summary>
        Points = 0x01,

        /// <summary>
        /// Requires at least 2 elements and 2 more to draw another primitive.
        /// </summary>
        Lines = 0x02,

        /// <summary>
        /// Requires at least 2 elements and 1 more to draw another primitive.
        /// </summary>
        LineStrip = 0x03,

        /// <summary>
        /// Requires at least 3 elements and 3 more to draw another primitive.
        /// </summary>
        Triangles = 0x04,

        /// <summary>
        /// Requires at least 3 elements and 1 more to draw another primitive.
        /// </summary>
        TriangleFan = 0x05,

        /// <summary>
        /// Requires at least 3 elements and 1 more to draw another primitive.
        /// </summary>
        TriangleStrip = 0x06,

        /// <summary>
        /// Requires at least 4 elements and 4 more to draw another primitive.
        /// </summary>
        LinesAdjacency = 0x0A,

        /// <summary>
        /// Requires at least 4 elements and 1 more to draw another primitive.
        /// </summary>
        LineStripAdjacency = 0x0B,

        /// <summary>
        /// Requires at least 6 elements and 6 more to draw another primitive.
        /// </summary>
        TrianglesAdjacency = 0x0C,

        /// <summary>
        /// Requires at least 6 elements and 2 more to draw another primitive.
        /// </summary>
        TriangleStripAdjacency = 0x0D,

        /// <summary>
        /// Requires at least 3 elements and 3 more to draw another primitive.
        /// </summary>
        Rects = 0x11,

        /// <summary>
        /// Requires at least 2 elements and 1 more to draw another primitive.
        /// </summary>
        LineLoop = 0x12,

        /// <summary>
        /// Requires at least 4 elements and 4 more to draw another primitive.
        /// </summary>
        Quads = 0x13,

        /// <summary>
        /// Requires at least 4 elements and 2 more to draw another primitive.
        /// </summary>
        QuadStrip = 0x14,

        /// <summary>
        /// Requires at least 2 elements and 2 more to draw another primitive.
        /// </summary>
        TessellateLines = 0x82,

        /// <summary>
        /// Requires at least 2 elements and 1 more to draw another primitive.
        /// </summary>
        TessellateLineStrip = 0x83,

        /// <summary>
        /// Requires at least 3 elements and 3 more to draw another primitive.
        /// </summary>
        TessellateTriangles = 0x84,

        /// <summary>
        /// Requires at least 3 elements and 1 more to draw another primitive.
        /// </summary>
        TessellateTriangleStrip = 0x86,

        /// <summary>
        /// Requires at least 4 elements and 4 more to draw another primitive.
        /// </summary>
        TessellateQuads = 0x93,

        /// <summary>
        /// Requires at least 4 elements and 2 more to draw another primitive.
        /// </summary>
        TessellateQuadStrip = 0x94
    }

    /// <summary>
    /// Represents the stencil function to be performed if stencil tests pass.
    /// </summary>
    public enum GX2StencilFunction
    {
        Keep,
        Zero,
        Replace,
        Increment,
        Decrement,
        Invert,
        IncrementWrap,
        DecrementWrap
    }

    /// <summary>
    /// Represents maximum desired anisotropic filter ratios. Higher ratios give better image quality, but slower
    /// performance.
    /// </summary>
    public enum GX2TexAnisoRatio
    {
        OneToOne,
        TwoToOne,
        FourToOne,
        EightToOne,
        SixteenToOne
    }

    /// <summary>
    /// Represents type of border color to use.
    /// </summary>
    public enum GX2TexBorderType
    {
        ClearBlack,
        SolidBlack,
        SolidWhite,
        UseRegister
    }

    /// <summary>
    /// Represents how to treat texture coordinates outside of the normalized coordinate texture range.
    /// </summary>
    public enum GX2TexClamp
    {
        Wrap,
        Mirror,
        Clamp,
        MirrorOnce,
        ClampHalfBorder,
        MirrorOnceHalfBorder,
        ClampBorder,
        MirrorOnceBorder
    }

    /// <summary>
    /// Represents desired texture filter options between mip levels.
    /// </summary>
    public enum GX2TexMipFilterType
    {
        NoMip,
        Point,
        Linear
    }

    /// <summary>
    /// Represents desired texture filter options within a plane.
    /// </summary>
    public enum GX2TexXYFilterType
    {
        Point,
        Bilinear
    }

    /// <summary>
    /// Represents desired texture filter options between Z planes.
    /// </summary>
    public enum GX2TexZFilterType
    {
        UseXY,
        Point,
        Linear
    }
}
