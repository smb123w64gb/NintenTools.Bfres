using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a parameter animation info in a <see cref="ShaderParamMatAnim"/> instance.
    /// </summary>
    [DebuggerDisplay(nameof(ParamAnimInfo) + " {" + nameof(Name) + "}")]
    public class ParamAnimInfo : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public ushort BeginCurve { get; set; }

        public ushort FloatCurveCount { get; set; }

        public ushort IntCurveCount { get; set; }

        public ushort BeginConstant { get; set; }

        public ushort ConstantCount { get; set; }

        public ushort SubBindIndex { get; set; }

        public string Name { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            BeginCurve = loader.ReadUInt16();
            FloatCurveCount = loader.ReadUInt16();
            IntCurveCount = loader.ReadUInt16();
            BeginConstant = loader.ReadUInt16();
            ConstantCount = loader.ReadUInt16();
            SubBindIndex = loader.ReadUInt16();
            Name = loader.GetName(loader.ReadOffset());
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }
}
