using Syroot.NintenTools.Bfres.Core;
using Syroot.NintenTools.Bfres.GX2;

namespace Syroot.NintenTools.Bfres
{
    public class VertexAttrib : ResContent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public VertexAttrib(ResFileLoader loader)
            : base(loader)
        {
            VertexAttribHead head = new VertexAttribHead(loader);
            Name = loader.GetName(head.OfsName);
            BufferIndex = head.IdxBuffer;
            Offset = head.Offset;
            Format = head.Format;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string Name { get; set; }

        public byte BufferIndex { get; set; }

        public ushort Offset { get; set; }

        public GX2AttribFormat Format { get; set; }
    }

    /// <summary>
    /// Represents the header of a <see cref="VertexAttrib"/> instance.
    /// </summary>
    internal class VertexAttribHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FVTX";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint OfsName;
        internal byte IdxBuffer;
        internal ushort Offset;
        internal GX2AttribFormat Format;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal VertexAttribHead(ResFileLoader loader)
        {
            OfsName = loader.ReadOffset();
            IdxBuffer = loader.ReadByte();
            loader.Seek(1);
            Offset = loader.ReadUInt16();
            Format = loader.ReadEnum<GX2AttribFormat>(true);
        }
    }
}