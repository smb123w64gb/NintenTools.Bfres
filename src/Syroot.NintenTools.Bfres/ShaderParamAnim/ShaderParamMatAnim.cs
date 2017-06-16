using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a material parameter animation in a <see cref="ShaderParamAnim"/> subfile.
    /// </summary>
    [DebuggerDisplay(nameof(ShaderParamMatAnim) + " {" + nameof(Name) + "}")]
    public class ShaderParamMatAnim : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name of the animated <see cref="Material"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the list of <see cref="ParamAnimInfo"/> instances.
        /// </summary>
        public IList<ParamAnimInfo> ParamAnimInfos { get; private set; }

        /// <summary>
        /// Gets <see cref="AnimCurve"/> instances animating properties of objects stored in this section.
        /// </summary>
        public IList<AnimCurve> Curves { get; private set; }

        public IList<AnimConstant> Constants { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ushort numAnimParam = loader.ReadUInt16();
            ushort numCurve = loader.ReadUInt16();
            ushort numConstant = loader.ReadUInt16();
            loader.Seek(2);
            int beginCurve = loader.ReadInt32();
            int beginParamAnim = loader.ReadInt32();
            Name = loader.LoadString();
            ParamAnimInfos = loader.LoadList<ParamAnimInfo>(numAnimParam);
            Curves = loader.LoadList<AnimCurve>(numCurve);
            Constants = loader.LoadCustom(() => loader.ReadAnimConstants(numConstant));
        }
        
        void IResData.Save(ResFileSaver saver)
        {
        }
    }
}