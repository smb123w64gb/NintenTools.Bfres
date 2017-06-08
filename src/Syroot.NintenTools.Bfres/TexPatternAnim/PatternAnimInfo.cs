using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a pattern animation info in a <see cref="TexPatternMatAnim"/> instance.
    /// </summary>
    [DebuggerDisplay(nameof(PatternAnimInfo) + " {" + nameof(Name) + "}")]
    public class PatternAnimInfo : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public sbyte CurveIndex;

        public sbyte SubBindIndex;

        public string Name;

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            CurveIndex = loader.ReadSByte();
            SubBindIndex = loader.ReadSByte();
            loader.Seek(2);
            Name = loader.GetName(loader.ReadOffset());
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }
}