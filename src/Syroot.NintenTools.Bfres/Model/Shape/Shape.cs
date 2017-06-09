using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FSHP section in a <see cref="Model"/> subfile.
    /// </summary>
    [DebuggerDisplay(nameof(Shape) + " {" + nameof(Name) + "}")]
    public class Shape : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{Shape}"/> instances.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_name != value)
                {
                    _name = value;
                    NameChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public ShapeFlags Flags { get; set; }

        public ushort MaterialIndex { get; set; }

        public ushort BoneIndex { get; set; }

        public ushort VertexBufferIndex { get; set; }

        public float Radius { get; set; }

        public IList<Mesh> Meshes { get; private set; }

        public IList<ushort> SkinBoneIndices { get; private set; }

        public IDictionary<string, KeyShape> KeyShapes { get; private set; }

        public IList<Bounding> SubMeshBoundings { get; private set; }

        public IList<BoundingNode> SubMeshBoundingNodes { get; private set; }

        public IList<ushort> SubMeshBoundingIndices { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ShapeHead head = new ShapeHead(loader);
            Name = loader.GetName(head.OfsName);
            Flags = head.Flags;
            MaterialIndex = head.IdxMaterial;
            BoneIndex = head.IdxBone;
            VertexBufferIndex = head.IdxVertexBuffer;
            Radius = head.Radius;
            Meshes = loader.LoadList<Mesh>(head.OfsMeshList, head.NumMesh);
            loader.Position = head.OfsSkinBoneIndexList;
            SkinBoneIndices = loader.ReadUInt16s(head.NumSkinBoneIndex);
            KeyShapes = loader.LoadDict<KeyShape>(head.OfsKeyShapeDict);
            loader.Position = head.OfsSubMeshBoundingList;
            SubMeshBoundings = loader.ReadBoundings(Meshes[0].SubMeshes.Count); // TODO: Validate count.

            // Normally nonexistent.
            if (head.NumSubMeshBoundingNodes != 0)
            {
                SubMeshBoundingNodes = loader.LoadList<BoundingNode>(head.OfsSubMeshBoundingNodeList,
                    head.NumSubMeshBoundingNodes);
                loader.Position = head.OfsSubMeshBoundingIndexList;
                SubMeshBoundingIndices = loader.ReadUInt16s(head.NumSubMeshBoundingNodes);
            }
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="Shape"/> instance.
    /// </summary>
    internal class ShapeHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSHP";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint OfsName;
        internal ShapeFlags Flags;
        internal ushort Idx;
        internal ushort IdxMaterial;
        internal ushort IdxBone;
        internal ushort IdxVertexBuffer;
        internal ushort NumSkinBoneIndex;
        internal byte NumVertexSkin;
        internal byte NumMesh;
        internal byte NumKeyShape;
        internal byte NumTargetAttrib;
        internal ushort NumSubMeshBoundingNodes; // Normally padding
        internal float Radius;
        internal uint OfsVertexBuffer;
        internal uint OfsMeshList;
        internal uint OfsSkinBoneIndexList;
        internal uint OfsKeyShapeDict;
        internal uint OfsSubMeshBoundingNodeList; // Normally nonexistent
        internal uint OfsSubMeshBoundingList;
        internal uint OfsSubMeshBoundingIndexList; // Normally nonexistent
        internal uint UserPointer;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ShapeHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            OfsName = loader.ReadOffset();
            Flags = loader.ReadEnum<ShapeFlags>(true);
            Idx = loader.ReadUInt16();
            IdxMaterial = loader.ReadUInt16();
            IdxBone = loader.ReadUInt16();
            IdxVertexBuffer = loader.ReadUInt16();
            NumSkinBoneIndex = loader.ReadUInt16();
            NumVertexSkin = loader.ReadByte();
            NumMesh = loader.ReadByte();
            NumKeyShape = loader.ReadByte();
            NumTargetAttrib = loader.ReadByte();
            NumSubMeshBoundingNodes = loader.ReadUInt16(); // Normally padding
            Radius = loader.ReadSingle();
            OfsVertexBuffer = loader.ReadOffset();
            OfsMeshList = loader.ReadOffset();
            OfsSkinBoneIndexList = loader.ReadOffset();
            OfsKeyShapeDict = loader.ReadOffset();
            if (NumSubMeshBoundingNodes == 0)
            {
                OfsSubMeshBoundingList = loader.ReadOffset();
            }
            else
            {
                OfsSubMeshBoundingNodeList = loader.ReadOffset(); // Normally nonexistent
                OfsSubMeshBoundingList = loader.ReadOffset();
                OfsSubMeshBoundingIndexList = loader.ReadOffset(); // Normally nonexistent
            }
            UserPointer = loader.ReadUInt32();
        }
    }

    public enum ShapeFlags : uint
    {
        HasVertexBuffer = 1 << 1
    }
}