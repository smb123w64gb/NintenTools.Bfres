using System.Collections.Generic;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    public class VertexBuffer : IResContent
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public byte VertexSkinCount { get; set; }

        public IList<VertexAttrib> Attributes { get; private set; }

        public IList<Buffer> Buffers { get; private set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public void Load(ResFileLoader loader)
        {
            VertexBufferHead head = new VertexBufferHead(loader);
            VertexSkinCount = head.NumVertexSkin;
            Attributes = loader.LoadDictList<VertexAttrib>(head.OfsVertexAttribDict);
            Buffers = loader.LoadList<Buffer>(head.OfsDataBufferList, head.NumDataBuffer);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="VertexBuffer"/> instance.
    /// </summary>
    internal class VertexBufferHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FVTX";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal byte NumVertexAttrib;
        internal byte NumDataBuffer;
        internal ushort Idx;
        internal uint NumVertex;
        internal byte NumVertexSkin;
        internal uint OfsVertexAttribList;
        internal uint OfsVertexAttribDict;
        internal uint OfsDataBufferList;
        internal uint UserPointer;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal VertexBufferHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            NumVertexAttrib = loader.ReadByte();
            NumDataBuffer = loader.ReadByte();
            Idx = loader.ReadUInt16();
            NumVertex = loader.ReadUInt32();
            NumVertexSkin = loader.ReadByte();
            loader.Seek(3);
            OfsVertexAttribList = loader.ReadOffset();
            OfsVertexAttribDict = loader.ReadOffset();
            OfsDataBufferList = loader.ReadOffset();
            UserPointer = loader.ReadUInt32();
        }
    }
}