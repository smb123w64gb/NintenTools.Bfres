using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    [DebuggerDisplay(nameof(ShaderParamAnim) + " {" + nameof(Name) + "}")]
    public class ShaderParamAnim : INamedResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string Name
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
        }
    }
}