using System;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents custom user variables which can be attached to many sections and subfiles of a <see cref="ResFile"/>.
    /// </summary>
    public class UserData : IResContent
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResContent.Load(ResFileLoader loader)
        {
        }
    }
}