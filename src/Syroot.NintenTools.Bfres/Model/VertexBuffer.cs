using System.Collections.Generic;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a data buffer holding vertices for a <see cref="Model"/> subfile.
    /// </summary>
    public class VertexBuffer : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public byte VertexSkinCount { get; set; }

        public INamedResDataList<VertexAttrib> Attributes { get; private set; }

        public IList<Buffer> Buffers { get; private set; }

        // TODO: Add methods to aid in retrieving strongly typed vertex data via attributes.

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            VertexBufferHead head = new VertexBufferHead(loader);
            using (loader.TemporarySeek())
            {
                VertexSkinCount = head.NumVertexSkin;
                Attributes = loader.LoadDictList<VertexAttrib>(head.OfsVertexAttribDict);
                Buffers = loader.LoadList<Buffer>(head.OfsBufferList, head.NumBuffer);
            }
        }

        void IResData.Reference(ResFileLoader loader)
        {
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
        internal byte NumBuffer;
        internal ushort Idx;
        internal uint NumVertex;
        internal byte NumVertexSkin;
        internal uint OfsVertexAttribList;
        internal uint OfsVertexAttribDict;
        internal uint OfsBufferList;
        internal uint UserPointer;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal VertexBufferHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            NumVertexAttrib = loader.ReadByte();
            NumBuffer = loader.ReadByte();
            Idx = loader.ReadUInt16();
            NumVertex = loader.ReadUInt32();
            NumVertexSkin = loader.ReadByte();
            loader.Seek(3);
            OfsVertexAttribList = loader.ReadOffset();
            OfsVertexAttribDict = loader.ReadOffset();
            OfsBufferList = loader.ReadOffset();
            UserPointer = loader.ReadUInt32();
        }
    }
}