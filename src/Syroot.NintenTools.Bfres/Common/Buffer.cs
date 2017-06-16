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
            uint dataPointer = loader.ReadUInt32();
            uint size = loader.ReadUInt32();
            uint handle = loader.ReadUInt32();
            Stride = loader.ReadUInt16();
            ushort numBuffering = loader.ReadUInt16();
            uint contextPointer = loader.ReadUInt32();
            Data = loader.LoadCustom(() => loader.ReadBytes((int)size * numBuffering));
        }
        
        void IResData.Save(ResFileSaver saver)
        {
        }
    }
}