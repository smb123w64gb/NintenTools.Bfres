using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a file attachment to a <see cref="ResFile"/> which can be of arbitrary data.
    /// </summary>
    public class ExternalFile : ResContent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public ExternalFile(ResFileLoader loader)
            : base(loader)
        {
            ExternalFileHead head = new ExternalFileHead(loader);
            loader.Seek(head.OfsData);
            Data = loader.ReadBytes((int)head.SizData);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public byte[] Data { get; set; }
    }

    /// <summary>
    /// Represents the header of a <see cref="ExternalFile"/> instance.
    /// </summary>
    internal class ExternalFileHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint OfsData;
        internal uint SizData;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ExternalFileHead(ResFileLoader loader)
        {
            OfsData = loader.ReadOffset();
            SizData = loader.ReadUInt32();
        }
    }
}