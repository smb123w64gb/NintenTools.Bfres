using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents the non-generic base of a dictionary which can quickly look up <see cref="IResData"/> instances of
    /// type <typeparamref name="T"/> via key or index.
    /// </summary>
    /// <typeparam name="T">The specialized type of the <see cref="IResData"/> instances.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(TypeProxy))]
    public abstract class ResDict : IEnumerable<KeyValuePair<string, IResData>>, IResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        protected IList<Node> _nodes; // Includes root node.

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        protected ResDict()
        {
            // Create root node.
            _nodes = new List<Node> { new Node() };
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the number of instances stored.
        /// </summary>
        public int Count
        {
            get { return _nodes.Count - 1; }
        }

        /// <summary>
        /// Gets all keys under which instances are stored.
        /// </summary>
        public IEnumerable<string> Keys
        {
            get
            {
                for (int i = 1; i < _nodes.Count; i++)
                {
                    yield return _nodes[i].Key;
                }
            }
        }

        /// <summary>
        /// Gets all stored instances.
        /// </summary>
        internal IEnumerable<IResData> Values
        {
            get
            {
                for (int i = 1; i < _nodes.Count; i++)
                {
                    yield return _nodes[i].Value;
                }
            }
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the <see cref="IResData"/> instance stored at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The 0-based index of the <see cref="IResData"/> instance to get or set.</param>
        /// <returns>The <see cref="IResData"/> at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is smaller than 0 or bigger or equal to
        /// <see cref="Count"/>.</exception>
        internal IResData this[int index]
        {
            get { return GetNode(index).Value; }
            set { GetNode(index).Value = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IResData"/> instance stored under the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The textual key of the <see cref="IResData"/> instance to get or set.</param>
        /// <returns>The <see cref="IResData"/> with the specified <paramref name="key"/>.</returns>
        /// <exception cref="ArgumentException">An <see cref="IResData"/> instance with the same <paramref name="key"/>
        /// already exists.</exception>
        /// <exception cref="KeyNotFoundException">An <see cref="IResData"/> instance with the given
        /// <paramref name="key"/> does not exist.</exception>
        internal IResData this[string key]
        {
            get { return GetNode(key).Value; }
            set { GetNode(key).Value = value; }
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Returns <c>true</c> if an <see cref="IResData"/> instance was stored under the given <paramref name="key"/>
        /// and has been assigned to <paramref name="value"/>, or <c>false</c> if no instance is stored under the
        /// given <paramref name="key"/> and <c>null</c> was assigned to <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The textual key of the <see cref="IResData"/> instance to get or set.</param>
        /// <param name="value">The variable receiving the found <see cref="IResData"/> or <c>null</c>.</param>
        /// <returns><c>true</c> if an instance was found and assigned; otherwise <c>false</c>.</returns>
        public bool TryGetValue(string key, out IResData value)
        {
            if (TryGetNode(key, out Node node))
            {
                value = node.Value;
                return true;
            }
            value = null;
            return false;
        }

        IEnumerator<KeyValuePair<string, IResData>> IEnumerable<KeyValuePair<string, IResData>>.GetEnumerator()
        {
            foreach (Node node in GetPublicNodes())
            {
                yield return new KeyValuePair<string, IResData>(node.Key, node.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (Node node in GetPublicNodes())
            {
                yield return new KeyValuePair<string, IResData>(node.Key, node.Value);
            }
        }

        void IResData.Load(ResFileLoader loader)
        {
            // Read the header.
            uint size = loader.ReadUInt32();
            int numNodes = loader.ReadInt32(); // Excludes root node.

            // Read the nodes including the root node.
            List<Node> nodes = new List<Node>();
            for (; numNodes >= 0; numNodes--)
            {
                nodes.Add(ReadNode(loader));
            }
            _nodes = nodes;
        }

        void IResData.Save(ResFileSaver saver)
        {
            // TODO: Update the radix tree values in the nodes.

            // Write header.
            saver.Write(sizeof(uint) * 2 + (_nodes.Count) * Node.SizeInBytes);
            saver.Write(Count);

            // Write nodes.
            int index = -1; // Start at -1 due to root node.
            foreach (Node node in _nodes)
            {
                saver.Write(node.Ref);
                saver.Write(node.IdxLeft);
                saver.Write(node.IdxRight);
                saver.SaveString(node.Key);
                switch (node.Value)
                {
                    case ResString resString:
                        saver.SaveString(resString);
                        break;
                    default:
                        saver.Save(node.Value, index++);
                        break;
                }
            }
        }

        // ---- METHODS (PROTECTED) ------------------------------------------------------------------------------------

        protected Node GetNode(int index)
        {
            // Prevent access to the root node.
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            return _nodes[index + 1];
        }

        protected Node GetNode(string key)
        {
            if (TryGetNode(key, out Node node))
            {
                return node;
            }
            throw new ArgumentException($"A node with the key \"{key}\" was not found.", nameof(key));
        }

        protected IEnumerable<Node> GetPublicNodes()
        {
            for (int i = 1; i < _nodes.Count; i++)
            {
                yield return _nodes[i];
            }
        }

        protected bool TryGetNode(string key, out Node node)
        {
            foreach (Node foundNode in GetPublicNodes())
            {
                if (foundNode.Key == key)
                {
                    node = foundNode;
                    return true;
                }
            }
            node = null;
            return false;
        }

        protected abstract IResData LoadNodeValue(ResFileLoader loader);

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private Node ReadNode(ResFileLoader loader)
        {
            return new Node()
            {
                Ref = loader.ReadUInt32(),
                IdxLeft = loader.ReadUInt16(),
                IdxRight = loader.ReadUInt16(),
                Key = loader.LoadString(),
                Value = LoadNodeValue(loader)
            };
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        protected class Node
        {
            internal const int SizeInBytes = 16;

            internal uint Ref;
            internal ushort IdxLeft;
            internal ushort IdxRight;
            internal string Key;
            internal IResData Value;
        }

        private class TypeProxy
        {
            private ResDict _dict;

            internal TypeProxy(ResDict dict)
            {
                _dict = dict;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePair<string, IResData>[] Items
            {
                get { return _dict.ToArray(); }
            }
        }
    }

    /// <summary>
    /// Represents a dictionary which can quickly look up <see cref="IResData"/> instances of type
    /// <typeparamref name="T"/> via key or index.
    /// </summary>
    /// <typeparam name="T">The specialized type of the <see cref="IResData"/> instances.</typeparam>
    public sealed class ResDict<T> : ResDict, IEnumerable<KeyValuePair<string, T>>
        where T : IResData, new()
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public ResDict() : base()
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets all stored instances.
        /// </summary>
        public new IEnumerable<T> Values
        {
            get
            {
                foreach (Node node in GetPublicNodes())
                {
                    yield return (T)node.Value;
                }
            }
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the instance stored at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The 0-based index of the instance to get or set.</param>
        /// <returns>The instance at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is smaller than 0 or bigger or equal to
        /// <see cref="Count"/>.</exception>
        public new T this[int index]
        {
            get { return (T)GetNode(index).Value; }
            set { GetNode(index).Value = value; }
        }

        /// <summary>
        /// Gets or sets the instance stored under the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The textual key of the instance to get or set.</param>
        /// <returns>The instance with the specified <paramref name="key"/>.</returns>
        /// <exception cref="ArgumentException">An instance with the same <paramref name="key"/> already exists.
        /// </exception>
        /// <exception cref="KeyNotFoundException">An instance with the given <paramref name="key"/> does not exist.
        /// </exception>
        public new T this[string key]
        {
            get { return (T)GetNode(key).Value; }
            set { GetNode(key).Value = value; }
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------
        
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            foreach (Node node in GetPublicNodes())
            {
                yield return new KeyValuePair<string, T>(node.Key, (T)node.Value);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if an instance was stored under the given <paramref name="key"/> and has been assigned
        /// to <paramref name="value"/>, or <c>false</c> if no instance is stored under the given <paramref name="key"/>
        /// and <c>null</c> was assigned to <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The textual key of the instance to get or set.</param>
        /// <param name="value">The variable receiving the found instance or <c>null</c>.</param>
        /// <returns><c>true</c> if an instance was found and assigned; otherwise <c>false</c>.</returns>
        public bool TryGetValue(string key, out T value)
        {
            if (TryGetNode(key, out Node node))
            {
                value = (T)node.Value;
                return true;
            }
            value = default(T);
            return false;
        }

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        protected override IResData LoadNodeValue(ResFileLoader loader)
        {
            return loader.Load<T>();
        }
    }
}
