using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Maths;

namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Loads the hierachy and data of a <see cref="ResFile"/>.
    /// </summary>
    public class ResFileLoader : BinaryDataReader
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _sizNode = 16;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ResFileLoader(ResFile resFile, Stream stream, bool leaveOpen = false)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
            ByteOrder = ByteOrder.BigEndian;
            ResFile = resFile;
        }

        internal ResFileLoader(ResFile resFile, string fileName)
            : this(resFile, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        internal ResFile ResFile { get; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Reads and returns the <see cref="String"/> from the given offset.
        /// </summary>
        /// <param name="offset">The total offset to read the <see cref="String"/> from.</param>
        /// <returns>The read <see cref="String"/>.</returns>
        internal string GetName(uint offset)
        {
            using (TemporarySeek(offset, SeekOrigin.Begin))
            {
                return ReadString(BinaryStringFormat.ZeroTerminated);
            }
        }
        
        /// <summary>
        /// Reads and returns <see cref="String"/> instances from the given <paramref name="offsets"/>.
        /// </summary>
        /// <param name="offsets">The total offsets to read the <see cref="String"/> instances from.</param>
        /// <returns>The read <see cref="String"/> instances.</returns>
        internal IList<string> GetNames(uint[] offsets)
        {
            string[] values = new string[offsets.Length];
            using (TemporarySeek())
            {
                for (int i = 0; i < offsets.Length; i++)
                {
                    Position = offsets[i];
                    values[i] = ReadString(BinaryStringFormat.ZeroTerminated);
                }
            }
            return values;
        }

        /// <summary>
        /// Reads an <see cref="IResData"/> instance from the given <paramref name="offset"/>. If the offset is 0,
        /// <c>null</c> is returned.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IResData"/> to load.</typeparam>
        /// <param name="offset">The offset to read the content from.</param>
        /// <returns>The new instance or <c>null</c>.</returns>
        internal T Load<T>(uint offset)
            where T : IResData, new()
        {
            if (offset == 0) return default(T);

            // Seek to the instance data and load it.
            Position = offset;
            return LoadInstance<T>();
        }

        /// <summary>
        /// Reads an <see cref="IList{String}"/> from the BFRES dictionary at given <paramref name="offset"/>. If the
        /// offset is 0, an empty list is returned. Names form the elements.
        /// </summary>
        /// <param name="offset">The offset at which to read the dictionary.</param>
        /// <returns>The read list.</returns>
        internal IList<string> LoadDictNames(uint offset)
        {
            List<string> list = new List<string>();
            if (offset == 0) return list;

            // Read the dictionary header.
            Position = offset;
            uint size = ReadUInt32();
            int entryCount = ReadInt32();

            // Read the dictionary nodes.
            Seek(_sizNode); // Skip the root node.
            for (; entryCount > 0; entryCount--)
            {
                uint keyBits = ReadUInt32();
                ushort idxLeft = ReadUInt16();
                ushort idxRight = ReadUInt16();
                uint ofsName = ReadOffset();
                uint ofsData = ReadOffset();
                list.Add(GetName(ofsData));
            }

            return list;
        }

        /// <summary>
        /// Reads an <see cref="IDictionary{String, T}"/> from the BFRES dictionary at given <paramref name="offset"/>.
        /// If the offset is 0, an empty dictionary is returned.
        /// </summary>
        /// <typeparam name="T">The type of the elements. Must implement <see cref="IResData"/>.</typeparam>
        /// <param name="offset">The offset at which to read the dictionary.</param>
        /// <returns>The read dictionary.</returns>
        internal IDictionary<string, T> LoadDict<T>(uint offset)
            where T : IResData, new()
        {
            SortedDictionary<string, T> dict = new SortedDictionary<string, T>();
            if (offset == 0) return dict;

            // Read the dictionary header.
            Position = offset;
            uint size = ReadUInt32();
            int entryCount = ReadInt32();

            // Read the dictionary nodes.
            Seek(_sizNode); // Skip the root node.
            for (; entryCount > 0; entryCount--)
            {
                uint keyBits = ReadUInt32();
                ushort idxLeft = ReadUInt16();
                ushort idxRight = ReadUInt16();
                uint ofsName = ReadOffset();
                uint ofsData = ReadOffset();
                // Seek to the data and instantiate it.
                using (TemporarySeek())
                {
                    dict.Add(GetName(ofsName), Load<T>(ofsData));
                }
            }

            return dict;
        }

        /// <summary>
        /// Reads an <see cref="IList{T}"/> from the BFRES dictionary at given <paramref name="offset"/>. If the offset
        /// is 0, an empty list is returned. Keys are not read as they are practically retrieved from instances upon
        /// saving.
        /// </summary>
        /// <typeparam name="T">The type of the elements. Must implement <see cref="IResData"/>.</typeparam>
        /// <param name="offset">The offset at which to read the dictionary.</param>
        /// <returns>The read list.</returns>
        internal INamedResDataList<T> LoadNamedDictList<T>(uint offset)
            where T : INamedResData, new()
        {
            NamedResDataList<T> list = new NamedResDataList<T>();
            if (offset == 0) return list;

            // Read the dictionary header.
            Position = offset;
            uint size = ReadUInt32();
            int entryCount = ReadInt32();

            // Read the dictionary nodes.
            Seek(_sizNode); // Skip the root node.
            for (; entryCount > 0; entryCount--)
            {
                uint keyBits = ReadUInt32();
                ushort idxLeft = ReadUInt16();
                ushort idxRight = ReadUInt16();
                uint ofsName = ReadOffset();
                uint ofsData = ReadOffset();
                // Seek to the data and instantiate it.
                using (TemporarySeek())
                {
                    list.Add(Load<T>(ofsData));
                }
            }

            return list;
        }

        /// <summary>
        /// Reads an <see cref="IList{T}"/> from the given <paramref name="offset"/> and element
        /// <paramref name="count"/>. If the offset is 0, an empty list is returned.
        /// </summary>
        /// <typeparam name="T">The type of the elements. Must implement <see cref="IResData"/>.</typeparam>
        /// <param name="offset">The offset at which to read the list.</param>
        /// <param name="count">The number of elements to read.</param>
        /// <returns>The read list.</returns>
        internal IList<T> LoadList<T>(uint offset, int count)
            where T : IResData, new()
        {
            List<T> list = new List<T>(count);
            if (offset == 0 || count == 0) return list;

            // Seek to the list start and read it.
            Position = offset;
            for (; count > 0; count--)
            {
                using (TemporarySeek())
                {
                    list.Add(LoadInstance<T>());
                }
            }

            return list;
        }

        /// <summary>
        /// Reads an <see cref="IList{T}"/> from the given <paramref name="offset"/> and element
        /// <paramref name="count"/>. If the offset is 0, an empty list is returned.
        /// </summary>
        /// <typeparam name="T">The type of the elements. Must implement <see cref="IResData"/>.</typeparam>
        /// <param name="offset">The offset at which to read the list.</param>
        /// <param name="count">The number of elements to read.</param>
        /// <returns>The read list.</returns>
        internal INamedResDataList<T> LoadNamedList<T>(uint offset, int count)
            where T : INamedResData, new()
        {
            NamedResDataList<T> list = new NamedResDataList<T>(count);
            if (offset == 0 || count == 0) return list;

            // Seek to the list start and read it.
            Position = offset;
            for (; count > 0; count--)
            {
                using (TemporarySeek())
                {
                    list.Add(LoadInstance<T>());
                }
            }

            return list;
        }

        /// <summary>
        /// Reads a <see cref="Bounding"/> instance from the current stream and returns it.
        /// </summary>
        /// <returns>The <see cref="Bounding"/> instance.</returns>
        internal Bounding ReadBounding()
        {
            return new Bounding()
            {
                Center = ReadVector3F(),
                Extent = ReadVector3F()
            };
        }

        /// <summary>
        /// Reads <see cref="Bounding"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Bounding"/> instances.</returns>
        internal IList<Bounding> ReadBoundings(int count)
        {
            Bounding[] values = new Bounding[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadBounding();
            }
            return values;
        }

        /// <summary>
        /// Reads a BFRES signature consisting of 4 ASCII characters encoded as an <see cref="UInt32"/> and checks for
        /// validity.
        /// </summary>
        /// <param name="validSignature">A valid signature.</param>
        /// <returns>The read signature.</returns>
        internal uint ReadSignature(string validSignature)
        {
            // Read the actual signature and compare it.
            string signature = ReadString(sizeof(uint), Encoding.ASCII);
            if (signature != validSignature)
            {
                throw new ResException($"Invalid signature, expected '{validSignature}' but got '{signature}'.");
            }
            return BitConverter.ToUInt32(Encoding.ASCII.GetBytes(signature), 0);
        }

        /// <summary>
        /// Reads a BFRES offset which is relative to itself, and returns the absolute address.
        /// </summary>
        /// <returns>The absolute address of the offset.</returns>
        internal uint ReadOffset()
        {
            uint offset = ReadUInt32();
            return offset == 0 ? 0 : (uint)Position - sizeof(uint) + offset;
        }

        /// <summary>
        /// Reads a <see cref="Matrix3x4"/> instance from the current stream and returns it.
        /// </summary>
        /// <returns>The <see cref="Matrix3x4"/> instance.</returns>
        internal Matrix3x4 ReadMatrix3x4()
        {
            return new Matrix3x4(
                ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(),
                ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(),
                ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        /// <summary>
        /// Reads <see cref="Matrix3x4"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Matrix3x4"/> instances.</returns>
        internal IList<Matrix3x4> ReadMatrix3x4s(int count)
        {
            Matrix3x4[] values = new Matrix3x4[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadMatrix3x4();
            }
            return values;
        }

        /// <summary>
        /// Reads a <see cref="Vector2"/> instance from the current stream and returns it.
        /// </summary>
        /// <returns>The <see cref="Vector2"/> instance.</returns>
        internal Vector2 ReadVector2()
        {
            return new Vector2(ReadInt32(), ReadInt32());
        }

        /// <summary>
        /// Reads <see cref="Vector2"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Vector2"/> instances.</returns>
        internal IList<Vector2> ReadVector2s(int count)
        {
            Vector2[] values = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadVector2();
            }
            return values;
        }

        /// <summary>
        /// Reads a <see cref="Vector2F"/> instance from the current stream and returns it.
        /// </summary>
        /// <returns>The <see cref="Vector2F"/> instance.</returns>
        internal Vector2F ReadVector2F()
        {
            return new Vector2F(ReadSingle(), ReadSingle());
        }

        /// <summary>
        /// Reads <see cref="Vector2F"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Vector2F"/> instances.</returns>
        internal IList<Vector2F> ReadVector2Fs(int count)
        {
            Vector2F[] values = new Vector2F[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadVector2F();
            }
            return values;
        }

        /// <summary>
        /// Reads a <see cref="Vector3"/> instance from the current stream and returns it.
        /// </summary>
        /// <returns>The <see cref="Vector3"/> instance.</returns>
        internal Vector3 ReadVector3()
        {
            return new Vector3(ReadInt32(), ReadInt32(), ReadInt32());
        }

        /// <summary>
        /// Reads <see cref="Vector3"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Vector3"/> instances.</returns>
        internal IList<Vector3> ReadVector3s(int count)
        {
            Vector3[] values = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadVector3();
            }
            return values;
        }

        /// <summary>
        /// Reads a <see cref="Vector3F"/> instance from the current stream and returns it.
        /// </summary>
        /// <returns>The <see cref="Vector3F"/> instance.</returns>
        internal Vector3F ReadVector3F()
        {
            return new Vector3F(ReadSingle(), ReadSingle(), ReadSingle());
        }

        /// <summary>
        /// Reads <see cref="Vector3F"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Vector3F"/> instances.</returns>
        internal IList<Vector3F> ReadVector3Fs(int count)
        {
            Vector3F[] values = new Vector3F[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadVector3F();
            }
            return values;
        }
        
        /// <summary>
        /// Reads a <see cref="Vector4"/> instance from the current stream and returns it.
        /// </summary>
        /// <returns>The <see cref="Vector4"/> instance.</returns>
        internal Vector4 ReadVector4()
        {
            return new Vector4(ReadInt32(), ReadInt32(), ReadInt32(), ReadInt32());
        }

        /// <summary>
        /// Reads <see cref="Vector4"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Vector4"/> instances.</returns>
        internal IList<Vector4> ReadVector4s(int count)
        {
            Vector4[] values = new Vector4[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadVector4();
            }
            return values;
        }

        /// <summary>
        /// Reads a <see cref="Vector4F"/> instance from the current stream and returns it.
        /// </summary>
        /// <returns>The <see cref="Vector4F"/> instance.</returns>
        internal Vector4F ReadVector4F()
        {
            return new Vector4F(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        /// <summary>
        /// Reads <see cref="Vector4F"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Vector4F"/> instances.</returns>
        internal IList<Vector4F> ReadVector4Fs(int count)
        {
            Vector4F[] values = new Vector4F[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadVector4F();
            }
            return values;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private T LoadInstance<T>()
            where T : IResData, new()
        {
            T instance = new T();
            instance.Load(this);
            return instance;
        }
    }
}
