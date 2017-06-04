using System;
using System.Collections.Generic;
using System.Text;

namespace Syroot.NintenTools.Bfres
{
    public class ResException : Exception
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public ResException(string message)
            : base(message)
        {
        }

        public ResException(string format, params object[] args)
            : base(String.Format(format, args))
        {
        }
    }
}
