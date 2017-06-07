using System.Diagnostics;
using System.Linq;

namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Represents a type proxy to display the elements of an <see cref="INamedResDataList{T}"/> instance in the Visual
    /// Studio debugging tools directly.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="INamedResData"/> elements.</typeparam>
    internal class NamedResDataListTypeProxy<T>
        where T : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private NamedResDataList<T> _list;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal NamedResDataListTypeProxy(NamedResDataList<T> list)
        {
            _list = list;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get { return _list.ToArray(); }
        }
    }
}
