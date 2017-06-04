using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Syroot.NintenTools.Bfres
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(NamedResDataListTypeProxy<>))]
    public class NamedResDataList<T> : INamedResDataList<T>
        where T : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private IList<T> _list;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public NamedResDataList()
        {
            _list = new List<T>();
        }

        public NamedResDataList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        public T this[int index]
        {
            get { return _list[index]; }
            set
            {
                ValidateNameExists(value.Name, index);
                _list[index] = value;
            }
        }

        public T this[string name]
        {
            get
            {
                foreach (T instance in _list)
                {
                    if (instance.Name == name) return instance;
                }
                throw new KeyNotFoundException("An element with the given name does not exist.");
            }
            set
            {
                // Find the existing element which has to be replaced.
                int index = IndexOf(value.Name);
                if (index == -1)
                {
                    throw new KeyNotFoundException("An element with the given name does not exist.");
                }
                ValidateNameExists(value.Name, index);
                _list[index] = value;
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public void Add(T item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public bool Contains(string name)
        {
            return CheckNameExists(name);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public int IndexOf(string name)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            CheckNameExists(item.Name);
            _list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public bool Remove(string name)
        {
            int index = IndexOf(name);
            if (index == -1)
            {
                return false;
            }
            _list.RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private bool CheckNameExists(string name, int ignoredIndex = -1)
        {
            // Check if another entry with the same name already exists.
            for (int i = 0; i < _list.Count; i++)
            {
                if (i != ignoredIndex && _list[i].Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        private void ValidateNameExists(string name, int ignoredIndex = -1)
        {
            if (CheckNameExists(name, ignoredIndex))
            {
                throw new ArgumentException("An element with the same name already exists in the list.");
            }
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
