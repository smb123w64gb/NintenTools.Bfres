using System.Collections.Generic;
using System.Diagnostics;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents the interface of a list referencing <see cref="INamedResData"/> instances, and allows looking them up
    /// via index or name.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public interface INamedResDataList<T> : IList<T>
        where T : INamedResData
    {
        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the element with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the element to get or set.</param>
        /// <returns>The element with the specified <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentException">An element with the same name already exists in the list.</exception>
        /// <exception cref="KeyNotFoundException">An element with the given <paramref name="name"/> does not exist.
        /// </exception>
        T this[string name] { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        bool Contains(string name);

        int IndexOf(string name);
        
        bool Remove(string item);
    }
}
