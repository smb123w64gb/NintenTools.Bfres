using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Maths;

namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Loads the hierachy and data of a <see cref="Bfres.ResFile"/>.
    /// </summary>
    public class ResFileLoader : BinaryDataReader
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private IDictionary<uint, IResData> _dataMap;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFileLoader"/> class loading data into the given
        /// <paramref name="resFile"/> from the specified <paramref name="stream"/> which is optionally left open.
        /// </summary>
        /// <param name="resFile">The <see cref="ResFile"/> instance to load data into.</param>
        /// <param name="stream">The <see cref="Stream"/> to read data from.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
        internal ResFileLoader(ResFile resFile, Stream stream, bool leaveOpen = false)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
            ByteOrder = ByteOrder.BigEndian;
            ResFile = resFile;
            _dataMap = new Dictionary<uint, IResData>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFileLoader"/> class from the file with the given
        /// <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        internal ResFileLoader(ResFile resFile, string fileName)
            : this(resFile, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the loaded <see cref="ResFile"/> instance.
        /// </summary>
        internal ResFile ResFile { get; }

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Starts deserializing the data from the <see cref="ResFile"/> root.
        /// </summary>
        internal void Execute()
        {
            // Load the raw data into structures recursively.
            ((IResData)ResFile).Load(this);
        }

        // ---- Data Load Methods ----

        /// <summary>
        /// Reads and returns an <see cref="IResData"/> instance of type <typeparamref name="T"/> from the following
        /// offset or returns <c>null</c> if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> to read.</typeparam>
        /// <returns>The <see cref="IResData"/> instance or <c>null</c>.</returns>
        [DebuggerStepThrough]
        internal T Load<T>()
            where T : IResData, new()
        {
            uint offset = ReadOffset();
            if (offset == 0) return default(T);

            // Seek to the instance data and load it.
            using (TemporarySeek(offset, SeekOrigin.Begin))
            {
                return ReadResData<T>();
            }
        }

        /// <summary>
        /// Reads and returns an instance of arbitrary type <typeparamref name="T"/> from the following offset with the
        /// given <paramref name="callback"/> or returns <c>null</c> if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the data to read.</typeparam>
        /// <param name="callback">The callback to read the instance data with.</param>
        /// <param name="offset">The optional offset to use instead of reading a following one.</param>
        /// <returns>The data instance or <c>null</c>.</returns>
        /// <remarks>Offset required for ExtFile header (offset specified before size).</remarks>
        [DebuggerStepThrough]
        internal T LoadCustom<T>(Func<T> callback, uint? offset = null)
        {
            offset = offset ?? ReadOffset();
            if (offset == 0) return default(T);

            using (TemporarySeek(offset.Value, SeekOrigin.Begin))
            {
                return callback.Invoke();
            }
        }

        /// <summary>
        /// Reads and returns an <see cref="ResDict{T}"/> instance with elements of type <typeparamref name="T"/> from
        /// the following offset or returns an empty instance if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> elements.</typeparam>
        /// <returns>The <see cref="ResDict{T}"/> instance.</returns>
        [DebuggerStepThrough]
        internal ResDict<T> LoadDict<T>()
            where T : IResData, new()
        {
            uint offset = ReadOffset();
            if (offset == 0) return new ResDict<T>();

            using (TemporarySeek(offset, SeekOrigin.Begin))
            {
                ResDict<T> dict = new ResDict<T>();
                ((IResData)dict).Load(this);
                return dict;
            }
        }

        /// <summary>
        /// Reads and returns an <see cref="IList{T}"/> instance with <paramref name="count"/> elements of type
        /// <typeparamref name="T"/> from the following offset or returns <c>null</c> if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> elements.</typeparam>
        /// <param name="offset">The optional offset to use instead of reading a following one.</param>
        /// <returns>The <see cref="IList{T}"/> instance or <c>null</c>.</returns>
        /// <remarks>Offset required for FMDL FVTX lists (offset specified before count).</remarks>
        [DebuggerStepThrough]
        internal IList<T> LoadList<T>(int count, uint? offset = null)
            where T : IResData, new()
        {
            List<T> list = new List<T>(count);
            offset = offset ?? ReadOffset();
            if (offset == 0 || count == 0) return list;

            // Seek to the list start and read it.
            using (TemporarySeek(offset.Value, SeekOrigin.Begin))
            {
                for (; count > 0; count--)
                {
                    list.Add(ReadResData<T>());
                }
                return list;
            }
        }

        /// <summary>
        /// Reads and returns a <see cref="String"/> instance from the following offset or <c>null</c> if the read
        /// offset is 0.
        /// </summary>
        /// <param name="encoding">The optional encoding of the text.</param>
        /// <returns>The read text.</returns>
        [DebuggerStepThrough]
        internal string LoadString(Encoding encoding = null)
        {
            uint offset = ReadOffset();
            if (offset == 0) return null;

            encoding = encoding ?? Encoding;
            using (TemporarySeek(offset, SeekOrigin.Begin))
            {
                return ReadString(BinaryStringFormat.ZeroTerminated, encoding);
            }
        }

        /// <summary>
        /// Reads and returns <paramref name="count"/> <see cref="String"/> instances from the following offsets.
        /// </summary>
        /// <param name="count">The number of instances to read.</param>
        /// <param name="encoding">The optional encoding of the texts.</param>
        /// <returns>The read texts.</returns>
        [DebuggerStepThrough]
        internal IList<string> LoadStrings(int count, Encoding encoding = null)
        {
            uint[] offsets = ReadOffsets(count);

            encoding = encoding ?? Encoding;
            string[] names = new string[offsets.Length];
            using (TemporarySeek())
            {
                for (int i = 0; i < offsets.Length; i++)
                {
                    uint offset = offsets[i];
                    if (offset == 0) continue;

                    Position = offset;
                    names[i] = ReadString(BinaryStringFormat.ZeroTerminated, encoding);
                }
                return names;
            }
        }

        // ---- Specialized Write Methods ----

        /// <summary>
        /// Reads a BFRES signature consisting of 4 ASCII characters encoded as an <see cref="UInt32"/> and checks for
        /// validity.
        /// </summary>
        /// <param name="validSignature">A valid signature.</param>
        /// <returns>The read signature.</returns>
        internal void CheckSignature(string validSignature)
        {
            // Read the actual signature and compare it.
            string signature = ReadString(sizeof(uint), Encoding.ASCII);
            if (signature != validSignature)
            {
                throw new ResException($"Invalid signature, expected '{validSignature}' but got '{signature}'.");
            }
        }

        /// <summary>
        /// Reads a <see cref="AnimConstant"/> instance from the current stream and returns it.
        /// </summary>
        /// <returns>The <see cref="AnimConstant"/> instance.</returns>
        internal AnimConstant ReadAnimConstant()
        {
            return new AnimConstant()
            {
                AnimDataOffset = ReadUInt32(),
                Value = ReadInt32()
            };
        }

        /// <summary>
        /// Reads a <see cref="AnimConstant"/> instance from the current stream and returns it.
        /// </summary>
        /// <returns>The <see cref="AnimConstant"/> instance.</returns>
        internal AnimConstant[] ReadAnimConstants(int count)
        {
            AnimConstant[] values = new AnimConstant[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadAnimConstant();
            }
            return values;
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

        internal Decimal10x5 ReadDecimal10x5()
        {
            return new Decimal10x5(ReadUInt16());
        }

        internal IList<Decimal10x5> ReadDecimal10x5s(int count)
        {
            Decimal10x5[] values = new Decimal10x5[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadDecimal10x5();
            }
            return values;
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
        /// Reads BFRES offsets which are relative to themselves, and returns the absolute addresses.
        /// </summary>
        /// <param name="count">The number of offsets to read.</param>
        /// <returns>The absolute addresses of the offsets.</returns>
        internal uint[] ReadOffsets(int count)
        {
            uint[] values = new uint[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadOffset();
            }
            return values;
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
        
        [DebuggerStepThrough]
        private T ReadResData<T>()
            where T : IResData, new()
        {
            uint offset = (uint)Position;

            // Same data can be referenced multiple times. Load it in any case to move in the stream, needed for lists.
            T instance = new T();
            instance.Load(this);

            // If possible, return an instance already representing the data.
            if (_dataMap.TryGetValue(offset, out IResData existingInstance))
            {
                return (T)existingInstance;
            }
            else
            {
                _dataMap.Add(offset, instance);
                return instance;
            }
        }
    }
}
