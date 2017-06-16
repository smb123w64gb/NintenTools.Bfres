using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a texture pattern material animation in a <see cref="TexPatternAnim"/> subfile.
    /// </summary>
    [DebuggerDisplay(nameof(TexPatternMatAnim) + " {" + nameof(Name) + "}")]
    public class TexPatternMatAnim : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets the name of the animated <see cref="Material"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the list of <see cref="PatternAnimInfo"/> instances.
        /// </summary>
        public IList<PatternAnimInfo> PatternAnimInfos { get; private set; }

        /// <summary>
        /// Gets <see cref="AnimCurve"/> instances animating properties of objects stored in this section.
        /// </summary>
        public IList<AnimCurve> Curves { get; private set; }

        /// <summary>
        /// Gets the initial <see cref="PatternAnimInfo"/> indices.
        /// </summary>
        public IList<ushort> BaseDataList { get; private set; }
        
        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ushort numPatAnim = loader.ReadUInt16();
            ushort numCurve = loader.ReadUInt16();
            int beginCurve = loader.ReadInt32();
            int beginPatAnim = loader.ReadInt32();
            Name = loader.LoadString();
            PatternAnimInfos = loader.LoadList<PatternAnimInfo>(numPatAnim);
            Curves = loader.LoadList<AnimCurve>(numCurve);
            BaseDataList = loader.LoadCustom(() => loader.ReadUInt16s(numPatAnim));
        }
        
        void IResData.Save(ResFileSaver saver)
        {
        }
    }
}