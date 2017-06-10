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
            VertexShapeAnimHead head = new VertexShapeAnimHead(loader);
            using (loader.TemporarySeek())
            {
                Name = loader.GetName(head.OfsName);
                KeyShapeAnimInfos = loader.LoadList<KeyShapeAnimInfo>(head.OfsKeyShapeAnimInfoList, head.NumKeyShapeAnim);
                Curves = loader.LoadList<AnimCurve>(head.OfsCurveList, head.NumCurve);

                if (head.OfsBaseDataList != 0)
                {
                    loader.Position = head.OfsBaseDataList;
                    BaseDataList = loader.ReadSingles(head.NumKeyShapeAnim - 1); // Does not store base shape values.
                }
            }
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="VertexShapeAnim"/> instance.
    /// </summary>
    internal class VertexShapeAnimHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal ushort NumCurve;
        internal ushort NumKeyShapeAnim;
        internal int BeginCurve; // First curve index relative to all.
        internal int BeginKeyShapeAnim; // First KeyShapeAnim index relative to all.
        internal uint OfsName;
        internal uint OfsKeyShapeAnimInfoList;
        internal uint OfsCurveList;
        internal uint OfsBaseDataList;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal VertexShapeAnimHead(ResFileLoader loader)
        {
            NumCurve = loader.ReadUInt16();
            NumKeyShapeAnim = loader.ReadUInt16();
            BeginCurve = loader.ReadInt32();
            BeginKeyShapeAnim = loader.ReadInt32();
            OfsName = loader.ReadOffset();
            OfsKeyShapeAnimInfoList = loader.ReadOffset();
            OfsCurveList = loader.ReadOffset();
            OfsBaseDataList = loader.ReadOffset();
        }
    }
}