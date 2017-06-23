using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FMDL subfile in a <see cref="ResFile"/>, storing model vertex data, skeletons and used materials.
    /// </summary>
    [DebuggerDisplay(nameof(Model) + " {" + nameof(Name) + "}")]
    public class Model : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FMDL";
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in <see cref="ResDict{Model}"/>
        /// instances.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the path of the file which originally supplied the data of this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets the <see cref="Skeleton"/> instance to deform the model with animations.
        /// </summary>
        public Skeleton Skeleton { get; set; }

        /// <summary>
        /// Gets the <see cref="VertexBuffer"/> instances storing the vertex data used by the <see cref="Shapes"/>.
        /// </summary>
        public IList<VertexBuffer> VertexBuffers { get; private set; }

        /// <summary>
        /// Gets the <see cref="Shape"/> instances forming the surface of the model.
        /// </summary>
        public ResDict<Shape> Shapes { get; private set; }

        /// <summary>
        /// Gets the <see cref="Material"/> instance applied on the <see cref="Shapes"/> to color their surface.
        /// </summary>
        public ResDict<Material> Materials { get; private set; }

        /// <summary>
        /// Gets customly attached <see cref="UserData"/> instances.
        /// </summary>
        public ResDict<UserData> UserData { get; private set; }

        /// <summary>
        /// Gets or sets the total number of vertices to process when drawing this model.
        /// </summary>
        // TODO: Compute total vertex count.
        public uint TotalVertexCount
        {
            get;
            private set;
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            loader.CheckSignature(_signature);
            Name = loader.LoadString();
            Path = loader.LoadString();
            Skeleton = loader.Load<Skeleton>();
            uint ofsVertexBufferList = loader.ReadOffset();
            Shapes = loader.LoadDict<Shape>();
            Materials = loader.LoadDict<Material>();
            UserData = loader.LoadDict<UserData>();
            ushort numVertexBuffer = loader.ReadUInt16();
            ushort numShape = loader.ReadUInt16();
            ushort numMaterial = loader.ReadUInt16();
            ushort numUserData = loader.ReadUInt16();
            TotalVertexCount = loader.ReadUInt32();
            uint userPointer = loader.ReadUInt32();

            VertexBuffers = loader.LoadList<VertexBuffer>(numVertexBuffer, ofsVertexBufferList);
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.SaveString(Name);
            saver.SaveString(Path);
            saver.Save(Skeleton);
            saver.SaveList(VertexBuffers);
            saver.SaveDict(Shapes);
            saver.SaveDict(Materials);
            saver.SaveDict(UserData);
            saver.Write((ushort)VertexBuffers.Count);
            saver.Write((ushort)Shapes.Count);
            saver.Write((ushort)Materials.Count);
            saver.Write((ushort)UserData.Count);
            saver.Write(TotalVertexCount);
            saver.Write(0); // UserPointer
        }
    }
}