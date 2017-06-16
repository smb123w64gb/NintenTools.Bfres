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

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedResDataList{T}"/> class.
        /// </summary>
        public NamedResDataList()
        {
            _list = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedResDataList{T}"/> class that has the specified initial
        /// <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public NamedResDataList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the element at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
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

        /// <summary>
        /// Gets or sets the element with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the element to get or set.</param>
        /// <returns>The element with the specified <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentException">An element with the same name already exists in the list.</exception>
        /// <exception cref="KeyNotFoundException">An element with the given <paramref name="name"/> does not exist.
        /// </exception>
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

        /// <summary>
        /// Gets the number of elements contained in the <see cref="INamedResDataList{T}"/>.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="INamedResDataList{T}"/> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Adds an <paramref name="item"/> to the <see cref="INamedResDataList{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="INamedResDataList{T}"/>.</param>
        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            item.NameChanged += Item_NameChanged;
            _list.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="INamedResDataList{T}"/>.
        /// </summary>
        public void Clear()
        {
            foreach (T instance in _list)
            {
                instance.NameChanged -= Item_NameChanged;
            }
            _list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="INamedResDataList{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="INamedResDataList{T}"/>.</param>
        /// <returns><c>true</c> if item is found in the <see cref="INamedResDataList{T}"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return _list.Contains(item);
        }

        /// <summary>
        /// Returns a value indicating whether the list contains an instance with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the instance to search for.</param>
        /// <returns><c>true</c> if an instance has the given name, otherwise <c>false</c>.</returns>
        public bool Contains(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return CheckNameExists(name);
        }

        /// <summary>
        /// Copies the elements of the <see cref="INamedResDataList{T}"/> to an <paramref name="array"/>, starting at a
        /// particular <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="INamedResDataList{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific <paramref name="item"/> in the <see cref="INamedResDataList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="INamedResDataList{T}"/>.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return _list.IndexOf(item);
        }

        /// <summary>
        /// Returns the zero-based index of the instance with the given <paramref name="name"/>, or <c>-1</c> if no
        /// instance has that name.
        /// </summary>
        /// <param name="name">The name of the instance to search for.</param>
        /// <returns>The zero-based index of the instance with the name, or <c>-1</c>.</returns>
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

        /// <summary>
        /// Inserts an item to the <see cref="INamedResDataList{T}"/> at the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="INamedResDataList{T}"/>.</param>
        public void Insert(int index, T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            CheckNameExists(item.Name);
            item.NameChanged += Item_NameChanged;
            _list.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific <paramref name="item"/> from the
        /// <see cref="INamedResDataList{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="INamedResDataList{T}"/>.</param>
        /// <returns><c>true</c> if the item was successfully removed from the <see cref="INamedResDataList{T}"/>;
        /// otherwise <c>false</c>. This method also returns <c>false</c> if the item is not found in the original
        /// <see cref="INamedResDataList{T}"/>.</returns>
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

        /// <summary>
        /// Removes the instance with the given <paramref name="name"/>. If it existed and was removed, <c>true</c> is
        /// returned, otherwise the list stays unchanged and <c>false</c> is returned.
        /// </summary>
        /// <param name="name">The name of the instance to search for.</param>
        /// <returns><c>true</c> if an instance was found and removed, otherwise <c>false</c>.</returns>
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

        /// <summary>
        /// Removes the <see cref="INamedResDataList{T}"/> item at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        /// <summary>
        /// Returns <c>true</c> when an instance with the given <paramref name="name"/> was found and assigned to
        /// <paramref name="value"/>, otherwise <c>false</c>.
        /// </summary>
        /// <param name="name">The name of the instance to search for.</param>
        /// <param name="value">The variable to assign a found instance to.</param>
        /// <returns><c>true</c> if an instance was found and assigned to <paramref name="value"/>, otherwise
        /// <c>false</c>.</returns>
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

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Required for <see cref="ResFileSaver"/> to work down queues of <see cref="NamedResDataList{T}"/> instances
        /// with unknown generic type.
        /// </summary>
        /// <returns>The sorted list holding only <see cref="INamedResData"/> instances.</returns>
        internal IList<INamedResData> AsSortedList()
        {
            List<INamedResData> list = new List<INamedResData>();
            foreach (INamedResData namedResData in this)
            {
                list.Add(namedResData);
            }
            list.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));
            return list;
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
