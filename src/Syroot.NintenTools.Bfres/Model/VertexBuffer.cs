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

        public byte VertexSkinCount { get; set; }

        public uint VertexCount { get; private set; } // TODO: Compute vertex count.

        public INamedResDataList<VertexAttrib> Attributes { get; private set; }

        public IList<Buffer> Buffers { get; private set; }

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
            Attributes = loader.LoadDictList<VertexAttrib>();
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
            saver.SaveList(Attributes);
            saver.SaveDictList(Attributes);
            saver.SaveList(Buffers);
            saver.Write(0); // UserPointer
        }
    }
}