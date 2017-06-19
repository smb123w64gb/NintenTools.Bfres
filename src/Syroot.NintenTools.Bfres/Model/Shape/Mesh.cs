using System.Collections.Generic;
using Syroot.NintenTools.Bfres.Core;
using Syroot.NintenTools.Bfres.GX2;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents the surface net of a <see cref="Shape"/> section, storing information on which
    /// index <see cref="Buffer"/> to use for referencing vertices of the shape, mostly used for different levels of
    /// detail (LoD) models.
    /// </summary>
    public class Mesh : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public GX2PrimitiveType PrimitiveType { get; set; }

        public GX2IndexFormat Format { get; set; }

        public uint ElementCount { get; set; }

        public IList<SubMesh> SubMeshes { get; private set; }

        public Buffer IndexBuffer { get; set;  }

        public uint Offset { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            PrimitiveType = loader.ReadEnum<GX2PrimitiveType>(true);
            Format = loader.ReadEnum<GX2IndexFormat>(true);
            ElementCount = loader.ReadUInt32();
            ushort numSubMesh = loader.ReadUInt16();
            loader.Seek(2);
            SubMeshes = loader.LoadList<SubMesh>(numSubMesh);
            IndexBuffer = loader.Load<Buffer>();
            Offset = loader.ReadUInt32();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.Write(PrimitiveType, true);
            saver.Write(Format, true);
            saver.Write(ElementCount);
            saver.Write((ushort)SubMeshes.Count);
            saver.Seek(2);
            saver.SaveList(SubMeshes);
            saver.Save(IndexBuffer);
            saver.Write(Offset);
        }
    }
}