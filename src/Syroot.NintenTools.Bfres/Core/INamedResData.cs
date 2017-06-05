using System;

namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Represents the common interface for <see cref="ResFile"/> structures storing a <see cref="Name"/> with which
    /// they can be referenced in lists.
    /// </summary>
    public interface INamedResData : IResData
    {
        // ---- EVENTS -------------------------------------------------------------------------------------------------

        event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        string Name { get; set; }
    }
}
