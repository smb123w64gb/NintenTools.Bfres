using System.IO;
using Syroot.BinaryData;
using Syroot.Maths;
using System.Text;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Saves the hierachy and data of a <see cref="Bfres.ResFile"/>.
    /// </summary>
    public class ResFileSaver : BinaryDataWriter
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        internal const int AlignmentSmall = 0x40; // Prominently seen with FVTX buffer data.

        private const int _dictNodeSize = 16;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private uint _ofsFileSize;
        private uint _ofsStringPool;
        private IDictionary<object, ItemEntry> _savedItems;
        private IList<KeyValuePair<object, ItemEntry>> _frameItems; // Iterate via index as modified in save loop.
        private IDictionary<string, List<uint>> _savedStrings;
        private IDictionary<object, BlockEntry> _savedBlocks;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFileSaver"/> class saving data from the given
        /// <paramref name="resFile"/> into the specified <paramref name="stream"/> which is optionally left open.
        /// </summary>
        /// <param name="resFile">The <see cref="ResFile"/> instance to save data from.</param>
        /// <param name="stream">The <see cref="Stream"/> to save data into.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
        internal ResFileSaver(ResFile resFile, Stream stream, bool leaveOpen)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
            ByteOrder = ByteOrder.BigEndian;
            ResFile = resFile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFileSaver"/> class for the file with the given
        /// <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The name of the file to save the data into.</param>
        internal ResFileSaver(ResFile resFile, string fileName)
            : this(resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), false)
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the saved <see cref="ResFile"/> instance.
        /// </summary>
        internal ResFile ResFile { get; }

        /// <summary>
        /// Gets the current index when writing lists or dicts.
        /// </summary>
        internal int CurrentIndex { get; private set; }

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Starts serializing the data from the <see cref="ResFile"/> root.
        /// </summary>
        internal void Execute()
        {
            // Create queues fetching the names for the string pool and data blocks to store behind the headers.
            _savedItems = new Dictionary<object, ItemEntry>();
            _frameItems = new List<KeyValuePair<object, ItemEntry>>();
            _savedStrings = new Dictionary<string, List<uint>>();
            _savedBlocks = new Dictionary<object, BlockEntry>();

            // Store the headers recursively and satisfy offsets to them, then the string pool and data blocks.
            WriteResData(ResFile);
            WriteOffsets();
            WriteStrings();
            WriteBlocks();
            // Save final file size into root header at the provided offset.
            Position = _ofsFileSize;
            Write((uint)BaseStream.Length);
            Flush();
        }

        // ---- Data Save Methods ----

        internal void SaveFieldFileSize()
        {
            _ofsFileSize = (uint)Position;
            Write(0);
        }

        internal void SaveFieldStringPool()
        {
            _ofsStringPool = (uint)Position;
            Write(0L);
        }

        internal void SaveList<T>(IList<T> list)
            where T : IResData
        {
            if (list.Count == 0)
            {
                Write(0);
                return;
            }

            // Also recognize lists by their first item (they should be written earlier than references to it).
            if (_savedItems.TryGetValue(list, out ItemEntry entry) || _savedItems.TryGetValue(list[0], out entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                entry = new ItemEntry((uint)Position, ItemEntryType.List);
                _savedItems.Add(list, entry);
                _frameItems.Add(new KeyValuePair<object, ItemEntry>(list, entry));
            }
            Write(entry.Target);
        }

        internal void SaveDict<T>(IDictionary<string, T> dict)
            where T : IResData
        {
            if (dict.Count == 0)
            {
                Write(0);
                return;
            }

            if (_savedItems.TryGetValue(dict, out ItemEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                entry = new ItemEntry((uint)Position, ItemEntryType.Dict);
                _savedItems.Add(dict, entry);
                _frameItems.Add(new KeyValuePair<object, ItemEntry>(dict, entry));
            }
            Write(entry.Target);
        }

        internal void SaveDictList<T>(INamedResDataList<T> dictList)
            where T : INamedResData
        {
            if (dictList.Count == 0)
            {
                Write(0);
                return;
            }

            if (_savedItems.TryGetValue(dictList, out ItemEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                entry = new ItemEntry((uint)Position, ItemEntryType.DictList);
                _savedItems.Add(dictList, entry);
                _frameItems.Add(new KeyValuePair<object, ItemEntry>(dictList, entry));
            }
            Write(entry.Target);
        }

        internal void SaveResData(IResData resData)
        {
            if (_savedItems.TryGetValue(resData, out ItemEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                entry = new ItemEntry((uint)Position, ItemEntryType.ResData);
                _savedItems.Add(resData, entry);
                _frameItems.Add(new KeyValuePair<object, ItemEntry>(resData, entry));
            }
            Write(entry.Target);
        }

        internal void SaveData(object data, Action callback)
        {
            if (_savedItems.TryGetValue(data, out ItemEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                entry = new ItemEntry((uint)Position, ItemEntryType.Data, callback);
                _savedItems.Add(data, entry);
                _frameItems.Add(new KeyValuePair<object, ItemEntry>(data, entry));
            }
            Write(entry.Target);
        }

        internal void SaveString(string str)
        {
            if (_savedStrings.TryGetValue(str, out List<uint> entryOffsets))
            {
                entryOffsets.Add((uint)Position);
            }
            else
            {
                _savedStrings.Add(str, new List<uint>(new uint[] { (uint)Position }));
            }
            Write(UInt32.MaxValue);
        }

        internal void SaveDataBlock(object data, int alignment, Action callback)
        {
            if (_savedBlocks.TryGetValue(data, out BlockEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                _savedBlocks.Add(data, new BlockEntry((uint)Position, alignment, callback));
            }
            Write(UInt32.MaxValue);
        }
        
        // ---- Specialized Write Methods ----

        /// <summary>
        /// Writes a <see cref="Matrix3x4"/> instance into the current stream.
        /// </summary>
        /// <param name="value">The <see cref="Matrix3x4"/> instance.</param>
        internal void Write(Matrix3x4 value)
        {
            Write(value.M11); Write(value.M12); Write(value.M13); Write(value.M14);
            Write(value.M21); Write(value.M22); Write(value.M23); Write(value.M24);
            Write(value.M31); Write(value.M32); Write(value.M33); Write(value.M34);
        }

        /// <summary>
        /// Writes <see cref="Matrix3x4"/> instances into the current stream.
        /// </summary>
        /// <param name="values">The <see cref="Matrix3x4"/> instances.</param>
        internal void Write(IEnumerable<Matrix3x4> values)
        {
            foreach (Matrix3x4 value in values)
            {
                Write(value);
            }
        }

        /// <summary>
        /// Writes a <see cref="Vector3"/> instance into the current stream.
        /// </summary>
        /// <param name="value">The <see cref="Vector3"/> instance.</param>
        internal void Write(Vector3 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }

        /// <summary>
        /// Writes <see cref="Vector3"/> instances into the current stream.
        /// </summary>
        /// <param name="values">The <see cref="Vector3"/> instances.</param>
        internal void Write(IEnumerable<Vector3> values)
        {
            foreach (Vector3 value in values)
            {
                Write(value);
            }
        }

        /// <summary>
        /// Writes a <see cref="Vector3F"/> instance into the current stream.
        /// </summary>
        /// <param name="value">The <see cref="Vector3F"/> instance.</param>
        internal void Write(Vector3F value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }

        /// <summary>
        /// Writes <see cref="Vector3F"/> instances into the current stream.
        /// </summary>
        /// <param name="values">The <see cref="Vector3F"/> instances.</param>
        internal void Write(IEnumerable<Vector3F> values)
        {
            foreach (Vector3F value in values)
            {
                Write(value);
            }
        }

        /// <summary>
        /// Writes a <see cref="Vector4F"/> instance into the current stream.
        /// </summary>
        /// <param name="value">The <see cref="Vector4F"/> instance.</param>
        internal void Write(Vector4F value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
            Write(value.W);
        }

        /// <summary>
        /// Writes <see cref="Vector4F"/> instances into the current stream.
        /// </summary>
        /// <param name="values">The <see cref="Vector4F"/> instances.</param>
        internal void Write(IEnumerable<Vector4F> values)
        {
            foreach (Vector4F value in values)
            {
                Write(value);
            }
        }

        /// <summary>
        /// Writes a BFRES signature consisting of 4 ASCII characters encoded as an <see cref="UInt32"/>.
        /// </summary>
        /// <param name="value">A valid signature.</param>
        internal void WriteSignature(string value)
        {
            Write(Encoding.ASCII.GetBytes(value));
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void WriteResData(IResData resData)
        {
            // Initiate a new frame for writing the instance.
            IList<KeyValuePair<object, ItemEntry>> previousItems = _frameItems;
            _frameItems = new List<KeyValuePair<object, ItemEntry>>();
            int previousIndex = CurrentIndex;

            // Store the header and let it queue items to follow it.
            resData.Save(this);

            // Store all queued items. Iterate via index as subsequent calls append to the frame list.
            SortedList<string, IResData> sorted;
            for (int i = 0; i < _frameItems.Count; i++)
            {
                KeyValuePair<object, ItemEntry> entry = _frameItems[i];

                // Remember the address at which this instance will be saved and start enumerables at index 0.
                entry.Value.Target = (uint)Position;
                CurrentIndex = 0;
                switch (entry.Value.Type)
                {
                    case ItemEntryType.List:
                        foreach (IResData element in (IList)entry.Key)
                        {
                            _savedItems.Add(element, new ItemEntry((uint)Position, ItemEntryType.ResData));
                            WriteResData(element);
                            CurrentIndex++;
                        }
                        break;

                    case ItemEntryType.Dict:
                        // Sort the dict ordinally.
                        sorted = new SortedList<string, IResData>(StringComparer.Ordinal);
                        foreach (DictionaryEntry element in (IDictionary)entry.Key)
                        {
                            sorted.Add((string)element.Key, (IResData)element.Value);
                        }
                        WriteDict(sorted);
                        break;

                    case ItemEntryType.DictList:
                        // Sort the dict ordinally.
                        sorted = new SortedList<string, IResData>(StringComparer.Ordinal);
                        foreach (INamedResData element in (IEnumerable)entry.Key)
                        {
                            sorted.Add(element.Name, element);
                        }
                        WriteDict(sorted);
                        break;

                    case ItemEntryType.ResData:
                        WriteResData((IResData)entry.Key);
                        break;

                    case ItemEntryType.Data:
                        entry.Value.Callback.Invoke();
                        break;
                }
            }
            
            // Restore the previous frame.
            _frameItems = previousItems;
            CurrentIndex = previousIndex;
        }

        private void WriteDict(SortedList<string, IResData> dict)
        {
            // Write header.
            Write(sizeof(uint) * 2 + (dict.Count + 1) * _dictNodeSize);
            Write(dict.Count);

            // Write nodes.
            WriteDictNode(UInt32.MaxValue, 1, 0, null, null); // Root node.
            foreach (KeyValuePair<string, IResData> entry in dict)
            {
                WriteDictNode(0x55555555, 0x6666, 0x7777, entry.Key, entry.Value); // TODO: Compute node values.
                CurrentIndex++;
            }
        }

        private void WriteDictNode(uint reference, ushort idxLeft, ushort idxRight, string name, IResData data)
        {
            Write(reference);
            Write(idxLeft);
            Write(idxRight);
            if (name == null) Write(0); else SaveString(name);
            if (data == null) Write(0); else SaveResData(data);
        }

        private void WriteStrings()
        {
            // Sort the strings ordinally.
            SortedList<string, List<uint>> sorted = new SortedList<string, List<uint>>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, List<uint>> entry in _savedStrings)
            {
                sorted.Add(entry.Key, entry.Value);
            }

            Align(4);
            uint stringPoolOffset = (uint)Position;

            foreach (KeyValuePair<string, List<uint>> entry in _savedStrings)
            {
                // Align and satisfy offsets.
                Write(entry.Key.Length);
                SatisfyOffsets(entry.Value);

                // Write the name.
                Write(entry.Key, BinaryStringFormat.ZeroTerminated, Encoding.UTF8);
                Align(4);
            }
            BaseStream.SetLength(Position); // Workaround to make last alignment expand the file if nothing follows.

            // Save string pool offset and size in main file header.
            uint stringPoolSize = (uint)(Position - stringPoolOffset);
            using (TemporarySeek(_ofsStringPool, SeekOrigin.Begin))
            {
                Write(stringPoolSize);
                Write((int)(stringPoolOffset - Position));
            }
        }

        private void WriteBlocks()
        {
            foreach (KeyValuePair<object, BlockEntry> entry in _savedBlocks)
            {
                // Align and satisfy offsets.
                if (entry.Value.Alignment != 0) Align(entry.Value.Alignment);
                SatisfyOffsets(entry.Value.Offsets);

                // Write the data.
                entry.Value.Callback.Invoke();
            }
        }

        private void WriteOffsets()
        {
            foreach (KeyValuePair<object, ItemEntry> entry in _savedItems)
            {
                SatisfyOffsets(entry.Value.Offsets, entry.Value.Target);
            }
        }

        private void SatisfyOffsets(IEnumerable<uint> offsets, uint? target = null)
        {
            target = target ?? (uint)Position;
            foreach (uint offset in offsets)
            {
                Position = offset;
                Write((int)(target - offset));
            }
            Position = target.Value;
        }

        // ---- STRUCTURES ---------------------------------------------------------------------------------------------

        internal enum ItemEntryType
        {
            List,
            Dict,
            DictList,
            ResData,
            Data
        }

        internal class ItemEntry
        {
            internal List<uint> Offsets;
            internal uint Target;
            internal ItemEntryType Type;
            internal Action Callback;

            internal ItemEntry(uint offset, ItemEntryType type, Action callback = null)
            {
                Offsets = new List<uint>(new uint[] { offset });
                Target = UInt32.MaxValue; // Recognize unsatisfied offsets easier.
                Type = type;
                Callback = callback;
            }
        }

        private class BlockEntry
        {
            internal List<uint> Offsets;
            internal int Alignment;
            internal Action Callback;

            internal BlockEntry(uint offset, int alignment, Action callback)
            {
                Offsets = new List<uint>(new uint[] { offset });
                Alignment = alignment;
                Callback = callback;
            }
        }
    }
}
