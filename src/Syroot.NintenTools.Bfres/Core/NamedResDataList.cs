using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Represents a list referencing <see cref="INamedResData"/> instances and allows indexed or named lookups.
    /// </summary>
    /// <remarks>Duplicate names possible through changing the name of an instance to the name of another instance
    /// stored in the same list are not prevented.</remarks>
    /// <typeparam name="T">The type of the elements.</typeparam>
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
                if (value == null) throw new ArgumentNullException(nameof(value));

                ValidateNameExists(value.Name, index);
                _list[index] = value;
            }
        }

        public T this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));

                foreach (T instance in _list)
                {
                    if (instance.Name == name) return instance;
                }
                throw new KeyNotFoundException("An element with the given name does not exist.");
            }
            set
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (value == null) throw new ArgumentNullException(nameof(value));

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
            if (item == null) throw new ArgumentNullException(nameof(item));

            item.NameChanged += Item_NameChanged;
            _list.Add(item);
        }

        public void Clear()
        {
            foreach (T instance in _list)
            {
                instance.NameChanged -= Item_NameChanged;
            }
            _list.Clear();
        }

        public bool Contains(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return _list.Contains(item);
        }

        public bool Contains(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

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
            if (item == null) throw new ArgumentNullException(nameof(item));

            return _list.IndexOf(item);
        }

        public int IndexOf(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

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
            if (item == null) throw new ArgumentNullException(nameof(item));

            CheckNameExists(item.Name);
            item.NameChanged += Item_NameChanged;
            _list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (_list.Remove(item))
            {
                item.NameChanged -= Item_NameChanged;
                return true;
            }
            return false;
        }

        public bool Remove(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (TryGetValue(name, out T instance))
            {
                instance.NameChanged -= Item_NameChanged;
                _list.Remove(instance);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public bool TryGetValue(string name, out T value)
        {
            foreach (T instance in _list)
            {
                if (instance.Name == name)
                {
                    value = instance;
                    return true;
                }
            }
            value = default(T);
            return false;
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

        // ---- EVENTHANDLERS ------------------------------------------------------------------------------------------

        private void Item_NameChanged(object sender, EventArgs e)
        {
            T instance = (T)sender;
            ValidateNameExists(instance.Name, IndexOf(instance));
        }
    }
}
