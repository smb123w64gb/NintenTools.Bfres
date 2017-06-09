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

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{T}"/> instances.
        /// </summary>
        string Name { get; set; }
    }
}
