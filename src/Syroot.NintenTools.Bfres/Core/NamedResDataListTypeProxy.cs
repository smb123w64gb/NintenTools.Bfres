using System.Diagnostics;
using System.Linq;

namespace Syroot.NintenTools.Bfres.Core
{
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
