using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a key shape animation info in a <see cref="VertexShapeAnim"/> instance.
    /// </summary>
    [DebuggerDisplay(nameof(ParamAnimInfo) + " {" + nameof(Name) + "}")]
    public class KeyShapeAnimInfo : IResData
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