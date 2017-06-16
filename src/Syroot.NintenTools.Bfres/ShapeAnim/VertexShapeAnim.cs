using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a vertex shape animation in a <see cref="ShapeAnim"/> subfile.
    /// </summary>
    [DebuggerDisplay(nameof(VertexShapeAnim) + " {" + nameof(Name) + "}")]
    public class VertexShapeAnim : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets or sets the name of the animated <see cref="Shape"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the list of <see cref="KeyShapeAnimInfo"/> instances.
        /// </summary>
        public IList<KeyShapeAnimInfo> KeyShapeAnimInfos { get; private set; }

        /// <summary>
        /// Gets <see cref="AnimCurve"/> instances animating properties of objects stored in this section.
        /// </summary>
        public IList<AnimCurve> Curves { get; private set; }

        /// <summary>
        /// Gets the list of base values, excluding the base shape (which is always being initialized with 0f).
        /// </summary>
        public float[] BaseDataList { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ushort numCurve = loader.ReadUInt16();
            ushort numKeyShapeAnim = loader.ReadUInt16();
            int beginCurve = loader.ReadInt32();
            int beginKeyShapeAnim = loader.ReadInt32();
            Name = loader.LoadString();
            KeyShapeAnimInfos = loader.LoadList<KeyShapeAnimInfo>(numKeyShapeAnim);
            Curves = loader.LoadList<AnimCurve>(numCurve);
            BaseDataList = loader.LoadCustom(() => loader.ReadSingles(numKeyShapeAnim - 1)); // Without base shape.
        }
        
        void IResData.Save(ResFileSaver saver)
        {
        }
    }
}