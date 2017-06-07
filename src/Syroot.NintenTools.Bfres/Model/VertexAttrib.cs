using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;
using Syroot.NintenTools.Bfres.GX2;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an attribute of a <see cref="VertexBuffer"/> describing the data format, type and layout of a
    /// specific data subset in the buffer.
    /// </summary>
    [DebuggerDisplay(nameof(VertexAttrib) + " {" + nameof(Name) + "}")]
    public class VertexAttrib : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_name != value)
                {
                    _name = value;
                    NameChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public byte BufferIndex { get; set; }

        public ushort Offset { get; set; }

        public GX2AttribFormat Format { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            VertexAttribHead head = new VertexAttribHead(loader);
            Name = loader.GetName(head.OfsName);
            BufferIndex = head.IdxBuffer;
            Offset = head.Offset;
            Format = head.Format;
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="VertexAttrib"/> instance.
    /// </summary>
    internal class VertexAttribHead
    {
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