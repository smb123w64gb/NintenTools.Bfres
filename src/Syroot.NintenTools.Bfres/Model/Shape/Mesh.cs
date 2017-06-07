using System;
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

        public IList<SubMesh> SubMeshes { get; private set; }

        public Buffer IndexBuffer { get; set;  }

        public uint Offset { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            MeshHead head = new MeshHead(loader);
            using (loader.TemporarySeek())
            {
                PrimitiveType = head.PrimitiveType;
                Format = head.Format;
                SubMeshes = loader.LoadList<SubMesh>(head.OfsSubMeshList, head.NumSubMesh);
                IndexBuffer = loader.Load<Buffer>(head.OfsIndexBuffer);
                Offset = head.Offset;
            }
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="Mesh"/> instance.
    /// </summary>
    internal class MeshHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal GX2PrimitiveType PrimitiveType;
        internal GX2IndexFormat Format;
        internal uint Count;
        internal ushort NumSubMesh;
        internal uint OfsSubMeshList;
        internal uint OfsIndexBuffer;
        internal uint Offset;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal MeshHead(ResFileLoader loader)
        {
            PrimitiveType = loader.ReadEnum<GX2PrimitiveType>(true);
            Format = loader.ReadEnum<GX2IndexFormat>(true);
            Count = loader.ReadUInt32();
            NumSubMesh = loader.ReadUInt16();
            loader.Seek(2);
            OfsSubMeshList = loader.ReadOffset();
            OfsIndexBuffer = loader.ReadOffset();
            Offset = loader.ReadOffset();
        }
    }
}