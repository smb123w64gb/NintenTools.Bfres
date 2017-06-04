using System;
using System.Diagnostics;
using System.Linq;

namespace Syroot.NintenTools.Bfres
{
    internal class NamedResDataListTypeProxy<T>
        where T : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private NamedResDataList<T> _list;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public NamedResDataListTypeProxy(NamedResDataList<T> list)
        {
            _list = list ?? throw new ArgumentNullException("collection");
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get { return _list.ToArray(); }
        }
    }
}
