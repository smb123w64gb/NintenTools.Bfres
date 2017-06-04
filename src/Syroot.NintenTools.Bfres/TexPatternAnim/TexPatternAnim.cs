using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    [DebuggerDisplay(nameof(TexPatternAnim) + " {" + nameof(Name) + "}")]
    public class TexPatternAnim : INamedResData
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