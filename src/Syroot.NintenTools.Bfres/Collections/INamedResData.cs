using System.Diagnostics;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents the common interface for <see cref="ResFile"/> structures storing a <see cref="Name"/> with which
    /// they can be referenced in lists.
    /// </summary>
    public interface INamedResData : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        string Name { get; set; }
    }
}
