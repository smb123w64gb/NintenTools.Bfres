using System.Collections;
using System.Collections.Generic;

namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Represents the interface of a list referencing <see cref="INamedResData"/> instances and allows indexed or named
    /// lookups.
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

        /// <summary>
        /// Returns a value indicating whether the list contains an instance with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the instance to search for.</param>
        /// <returns><c>true</c> if an instance has the given name, otherwise <c>false</c>.</returns>
        bool Contains(string name);

        /// <summary>
        /// Returns the zero-based index of the instance with the given <paramref name="name"/>, or <c>-1</c> if no
        /// instance has that name.
        /// </summary>
        /// <param name="name">The name of the instance to search for.</param>
        /// <returns>The zero-based index of the instance with the name, or <c>-1</c>.</returns>
        int IndexOf(string name);

        /// <summary>
        /// Removes the instance with the given <paramref name="name"/>. If it existed and was removed, <c>true</c> is
        /// returned, otherwise the list stays unchanged and <c>false</c> is returned.
        /// </summary>
        /// <param name="name">The name of the instance to search for.</param>
        /// <returns><c>true</c> if an instance was found and removed, otherwise <c>false</c>.</returns>
        bool Remove(string name);

        /// <summary>
        /// Returns <c>true</c> when an instance with the given <paramref name="name"/> was found and assigned to
        /// <paramref name="value"/>, otherwise <c>false</c>.
        /// </summary>
        /// <param name="name">The name of the instance to search for.</param>
        /// <param name="value">The variable to assign a found instance to.</param>
        /// <returns><c>true</c> if an instance was found and assigned to <paramref name="value"/>, otherwise
        /// <c>false</c>.</returns>
        bool TryGetValue(string name, out T value);
    }
}
