using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a buffer of data uploaded to the GX2 GPU which can hold arbitrary data.
    /// </summary>
    public class Buffer : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// The size of a full vertex in bytes.
        /// </summary>
        public ushort Stride { get; set; }
        
        /// <summary>
        /// The raw bytes stored.
        /// </summary>
        public byte[] Data { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            BufferHead head = new BufferHead(loader);
            using (loader.TemporarySeek())
            {
                Stride = head.Stride;
                if (head.OfsData != 0)
                {
                    loader.Position = head.OfsData;
                    Data = loader.ReadBytes((int)head.Size * head.NumBuffering);
                }
            }
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="Buffer"/> instance.
    /// </summary>
    internal class BufferHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint DataPointer;
        internal uint Size;
        internal uint Handle;
        internal ushort Stride;
        internal ushort NumBuffering;
        internal uint ContextPointer;
        internal uint OfsData;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal BufferHead(ResFileLoader loader)
        {
            DataPointer = loader.ReadUInt32();
            Size = loader.ReadUInt32();
            Handle = loader.ReadUInt32();
            Stride = loader.ReadUInt16();
            NumBuffering = loader.ReadUInt16();
            ContextPointer = loader.ReadUInt32();
            OfsData = loader.ReadOffset();
        }
    }
}