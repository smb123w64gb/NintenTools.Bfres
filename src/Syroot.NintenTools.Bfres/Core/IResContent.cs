using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Syroot.NintenTools.Bfres.Core
{
    public interface IResContent
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        void Load(ResFileLoader loader);
    }
}
