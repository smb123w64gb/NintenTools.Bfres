using System.Collections.Generic;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a data buffer holding vertices for a <see cref="Model"/> subfile.
    /// </summary>
    public class VertexBuffer : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FVTX";

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the number of bones influencing the vertices stored in this buffer. 0 influences equal
        /// rigidbodies (no skinning), 1 equal rigid skinning and 2 smooth skinning.
        /// </summary>
        public byte VertexSkinCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of vertices resulting from the <see cref="Buffers"/>.
        /// </summary>
        public uint VertexCount { get; set; } // TODO: Compute vertex count.

        /// <summary>
        /// Gets or sets the dictionary of <see cref="VertexAttrib"/> instances describing how to interprete data in the
        /// <see cref="Buffers"/>.
        /// </summary>
        public ResDict<VertexAttrib> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="Buffer"/> instances storing raw unformatted vertex data.
        /// </summary>
        public IList<Buffer> Buffers { get; set; }

        // TODO: Add methods to aid in retrieving strongly typed vertex data via attributes.

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            loader.CheckSignature(_signature);
            byte numVertexAttrib = loader.ReadByte();
            byte numBuffer = loader.ReadByte();
            ushort idx = loader.ReadUInt16();
            VertexCount = loader.ReadUInt32();
            VertexSkinCount = loader.ReadByte();
            loader.Seek(3);
            uint ofsVertexAttribList = loader.ReadOffset(); // Only load dict.
            Attributes = loader.LoadDict<VertexAttrib>();
            Buffers = loader.LoadList<Buffer>(numBuffer);
            uint userPointer = loader.ReadUInt32();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.Write((byte)Attributes.Count);
            saver.Write((byte)Buffers.Count);
            saver.Write((ushort)saver.CurrentIndex);
            saver.Write(VertexCount);
            saver.Write(VertexSkinCount);
            saver.Seek(3);
            saver.SaveList(Attributes.Values);
            saver.SaveDict(Attributes);
            saver.SaveList(Buffers);
            saver.Write(0); // UserPointer
        }
    }
}