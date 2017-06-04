using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a buffer of data uploaded to the GX2 GPU which can hold arbitrary data.
    /// </summary>
    public class Buffer : IResContent
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public ushort Stride { get; set; }
        
        public byte[] Data { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public void Load(ResFileLoader loader)
        {
            BufferHead head = new BufferHead(loader);
            Stride = head.Stride;
            loader.Position = head.OfsData;
            Data = loader.ReadBytes((int)head.Size * head.NumBuffering);
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