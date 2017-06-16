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
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSHP";

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

        public VertexBuffer VertexBuffer { get; private set; }

        public IList<Mesh> Meshes { get; private set; }

        public IList<ushort> SkinBoneIndices { get; private set; }

        public IDictionary<string, KeyShape> KeyShapes { get; private set; }

        public IList<Bounding> SubMeshBoundings { get; private set; }

        public IList<BoundingNode> SubMeshBoundingNodes { get; private set; }

        public IList<ushort> SubMeshBoundingIndices { get; private set; }
        
        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            loader.CheckSignature(_signature);
            Name = loader.LoadString();
            Flags = loader.ReadEnum<ShapeFlags>(true);
            ushort idx = loader.ReadUInt16();
            MaterialIndex = loader.ReadUInt16();
            BoneIndex = loader.ReadUInt16();
            VertexBufferIndex = loader.ReadUInt16();
            ushort numSkinBoneIndex = loader.ReadUInt16();
            byte numVertexSkin = loader.ReadByte();
            byte numMesh = loader.ReadByte();
            byte numKeyShape = loader.ReadByte();
            byte numTargetAttrib = loader.ReadByte();
            ushort numSubMeshBoundingNodes = loader.ReadUInt16(); // Normally padding
            Radius = loader.ReadSingle();
            VertexBuffer = loader.Load<VertexBuffer>();
            Meshes = loader.LoadList<Mesh>(numMesh);
            SkinBoneIndices = loader.LoadCustom(() => loader.ReadUInt16s(numSkinBoneIndex));
            KeyShapes = loader.LoadDict<KeyShape>();
            if (numSubMeshBoundingNodes == 0)
            {
                // TODO: Validate count.
                SubMeshBoundings = loader.LoadCustom(() => loader.ReadBoundings(Meshes[0].SubMeshes.Count)); 
            }
            else
            {
                // Normally nonexistent
                SubMeshBoundingNodes = loader.LoadList<BoundingNode>(numSubMeshBoundingNodes);
                // TODO: Validate count.
                SubMeshBoundings = loader.LoadCustom(() => loader.ReadBoundings(Meshes[0].SubMeshes.Count));
                // Normally nonexistent
                SubMeshBoundingIndices = loader.LoadCustom(() => loader.ReadUInt16s(numSubMeshBoundingNodes));
            }
            uint userPointer = loader.ReadUInt32();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
        }
    }
    
    public enum ShapeFlags : uint
    {
        HasVertexBuffer = 1 << 1
    }
}