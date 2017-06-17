using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Maths;

namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Saves the hierachy and data of a <see cref="Bfres.ResFile"/>.
    /// </summary>
    public class ResFileSaver : BinaryDataWriter
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets a data block alignment typically seen with <see cref="Buffer.Data"/>.
        /// </summary>
        internal const uint AlignmentSmall = 0x40;

        private const int _dictNodeSize = 16;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private uint _ofsFileSize;
        private uint _ofsStringPool;
        private IDictionary<object, ItemEntry> _savedItems;
        private IList<KeyValuePair<object, ItemEntry>> _frameItems; // Iterate via index as modified in save loop.
        private IDictionary<string, StringEntry> _savedStrings;
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
            _savedStrings = new Dictionary<string, StringEntry>();
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

        /// <summary>
        /// Reserves space for an offset to the <paramref name="resData"/> written later.
        /// </summary>
        /// <param name="resData">The <see cref="IResData"/> to save.</param>
        internal void Save(IResData resData)
        {
            Save(resData, -1);
        }

        /// <summary>
        /// Reserves space for the <see cref="ResFile"/> file size field which is automatically filled later.
        /// </summary>
        internal void SaveFieldFileSize()
        {
            _ofsFileSize = (uint)Position;
            Write(0);
        }

        /// <summary>
        /// Reserves space for the <see cref="ResFile"/> string pool size and offset fields which are automatically
        /// filled later.
        /// </summary>
        internal void SaveFieldStringPool()
        {
            _ofsStringPool = (uint)Position;
            Write(0L);
        }

        /// <summary>
        /// Reserves space for an offset to the <paramref name="list"/> written later.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> elements.</typeparam>
        /// <param name="list">The <see cref="IList{T}"/> to save.</param>
        internal void SaveList<T>(IList<T> list)
            where T : IResData
        {
            if (list.Count == 0)
            {
                Write(0);
                return;
            }

            // Also recognize lists by their first item (they should be written earlier than references to it).
            if (_savedItems.TryGetValue(list[0], out ItemEntry entry) || _savedItems.TryGetValue(list, out entry))
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

        /// <summary>
        /// Reserves space for an offset to the <paramref name="dict"/> written later.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> element values.</typeparam>
        /// <param name="list">The <see cref="IDictionary{String, T}"/> to save.</param>
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

        /// <summary>
        /// Reserves space for an offset to the <paramref name="dictList"/> written later.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="INamedResData"/> element values.</typeparam>
        /// <param name="list">The <see cref="INamedResDataList{T}"/> to save.</param>
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
                // Also remember the first item to make it an offset for lists referencing the dict elements.
                entry = new ItemEntry(ItemEntryType.ResData);
                _savedItems.Add(dictList[0], entry);
                _frameItems.Add(new KeyValuePair<object, ItemEntry>(dictList[0], entry));
            }
            Write(entry.Target);
        }

        /// <summary>
        /// Reserves space for an offset to the <paramref name="dictNames"/> written later.
        /// </summary>
        /// <param name="list">The <see cref="IList{String}"/> to save.</param>
        internal void SaveDictNames(IList<string> dictNames)
        {
            if (dictNames.Count == 0)
            {
                Write(0);
                return;
            }

            if (_savedItems.TryGetValue(dictNames, out ItemEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                entry = new ItemEntry((uint)Position, ItemEntryType.DictNames);
                _savedItems.Add(dictNames, entry);
                _frameItems.Add(new KeyValuePair<object, ItemEntry>(dictNames, entry));
            }
            Write(entry.Target);
        }

        /// <summary>
        /// Reserves space for an offset to the <paramref name="data"/> written later with the
        /// <paramref name="callback"/>.
        /// </summary>
        /// <param name="data">The data to save.</param>
        /// <param name="callback">The <see cref="Action"/> to invoke to write the data.</param>
        internal void SaveCustom(object data, Action callback)
        {
            if (_savedItems.TryGetValue(data, out ItemEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                entry = new ItemEntry((uint)Position, ItemEntryType.Data, -1, callback);
                _savedItems.Add(data, entry);
                _frameItems.Add(new KeyValuePair<object, ItemEntry>(data, entry));
            }
            Write(entry.Target);
        }

        /// <summary>
        /// Reserves space for an offset to the <paramref name="str"/> written later in the string pool with the
        /// specified <paramref name="encoding"/>.
        /// </summary>
        /// <param name="str">The name to save.</param>
        /// <param name="encoding">The <see cref="Encoding"/> in which the name will be stored.</param>
        internal void SaveString(string str, Encoding encoding = null)
        {
            if (_savedStrings.TryGetValue(str, out StringEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                _savedStrings.Add(str, new StringEntry((uint)Position, encoding));
            }
            Write(UInt32.MaxValue);
        }

        /// <summary>
        /// Reserves space for offsets to the <paramref name="strings"/> written later in the string pool with the
        /// specified <paramref name="encoding"/>
        /// </summary>
        /// <param name="strings">The names to save.</param>
        /// <param name="encoding">The <see cref="Encoding"/> in which the names will be stored.</param>
        internal void SaveStrings(IEnumerable<string> strings, Encoding encoding = null)
        {
            foreach (string str in strings)
            {
                SaveString(str, encoding);
            }
        }

        /// <summary>
        /// Reserves space for an offset to the <paramref name="data"/> written later in the data block pool.
        /// </summary>
        /// <param name="data">The data to save.</param>
        /// <param name="alignment">The alignment to seek to before invoking the callback.</param>
        /// <param name="callback">The <see cref="Action"/> to invoke to write the data.</param>
        internal void SaveBlock(object data, uint alignment, Action callback)
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
        /// Writes a BFRES signature consisting of 4 ASCII characters encoded as an <see cref="UInt32"/>.
        /// </summary>
        /// <param name="value">A valid signature.</param>
        internal void WriteSignature(string value)
        {
            Write(Encoding.ASCII.GetBytes(value));
        }

        /// <summary>
        /// Writes a <see cref="AnimConstant"/> instance into the current stream.
        /// </summary>
        /// <param name="value">The <see cref="AnimConstant"/> instance.</param>
        internal void Write(AnimConstant value)
        {
            Write(value.AnimDataOffset);
            Write(value.Value);
        }

        /// <summary>
        /// Writes <see cref="AnimConstant"/> instances into the current stream.
        /// </summary>
        /// <param name="values">The <see cref="AnimConstant"/> instances.</param>
        internal void Write(IEnumerable<AnimConstant> values)
        {
            foreach (AnimConstant value in values)
            {
                Write(value);
            }
        }

        /// <summary>
        /// Writes a <see cref="Bounding"/> instance into the current stream.
        /// </summary>
        /// <param name="value">The <see cref="Bounding"/> instance.</param>
        internal void Write(Bounding value)
        {
            Write(value.Center);
            Write(value.Extent);
        }

        /// <summary>
        /// Writes <see cref="Bounding"/> instances into the current stream.
        /// </summary>
        /// <param name="values">The <see cref="Bounding"/> instances.</param>
        internal void Write(IEnumerable<Bounding> values)
        {
            foreach (Bounding value in values)
            {
                Write(value);
            }
        }

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
        /// Writes a <see cref="Vector2F"/> instance into the current stream.
        /// </summary>
        /// <param name="value">The <see cref="Vector2F"/> instance.</param>
        internal void Write(Vector2F value)
        {
            Write(value.X);
            Write(value.Y);
        }

        /// <summary>
        /// Writes <see cref="Vector2F"/> instances into the current stream.
        /// </summary>
        /// <param name="values">The <see cref="Vector2F"/> instances.</param>
        internal void Write(IEnumerable<Vector2F> values)
        {
            foreach (Vector2F value in values)
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

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Save(IResData resData, int index)
        {
            if (resData == null)
            {
                Write(0);
                return;
            }

            if (_savedItems.TryGetValue(resData, out ItemEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                entry = new ItemEntry((uint)Position, ItemEntryType.ResData, index);
                _savedItems.Add(resData, entry);
                _frameItems.Add(new KeyValuePair<object, ItemEntry>(resData, entry));
            }
            Write(entry.Target);
        }

        private void WriteResData(IResData resData)
        {
            // Initiate a new frame for writing the instance.
            IList<KeyValuePair<object, ItemEntry>> previousItems = _frameItems;
            _frameItems = new List<KeyValuePair<object, ItemEntry>>();

            // Store the header and let it queue items to follow it.
            resData.Save(this);

            // Store all queued items. Iterate via index as subsequent calls append to the frame list.
            SortedList<string, IResData> sorted;
            for (int i = 0; i < _frameItems.Count; i++)
            {
                KeyValuePair<object, ItemEntry> entry = _frameItems[i];
                CurrentIndex = entry.Value.Index;

                // Remember the address at which this instance will be saved.
                entry.Value.Target = (uint)Position;
                switch (entry.Value.Type)
                {
                    case ItemEntryType.List:
                        int index = 0;
                        foreach (IResData element in (IEnumerable)entry.Key)
                        {
                            _savedItems.Add(element, new ItemEntry((uint)Position, ItemEntryType.ResData, index++));
                            WriteResData(element);
                        }
                        break;

                    case ItemEntryType.Dict:
                        // Sort the dict ordinally.
                        sorted = new SortedList<string, IResData>(ResStringComparer.Instance);
                        foreach (DictionaryEntry element in (IDictionary)entry.Key)
                        {
                            sorted.Add((string)element.Key, (IResData)element.Value);
                        }
                        WriteDict(sorted);
                        break;

                    case ItemEntryType.DictList:
                        // Sort the dict ordinally.
                        sorted = new SortedList<string, IResData>(ResStringComparer.Instance);
                        foreach (INamedResData element in (IEnumerable)entry.Key)
                        {
                            sorted.Add(element.Name, element);
                        }
                        WriteDict(sorted);
                        break;

                    case ItemEntryType.DictNames:
                        WriteDictNames((List<string>)entry.Key);
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
        }

        private void WriteDict(SortedList<string, IResData> dict)
        {
            // Write header.
            Write(sizeof(uint) * 2 + (dict.Count + 1) * _dictNodeSize);
            Write(dict.Count);

            // Write nodes.
            WriteDictNode(UInt32.MaxValue, 1, 0, null, null); // Root node.
            int i = 0;
            foreach (KeyValuePair<string, IResData> entry in dict)
            {
                WriteDictNode(0x55555555, 0x6666, 0x7777, entry.Key, entry.Value, i++); // TODO: Compute node values.
            }
        }

        private void WriteDictNode(uint reference, ushort idxLeft, ushort idxRight, string name, IResData data,
            int index = -1)
        {
            Write(reference);
            Write(idxLeft);
            Write(idxRight);
            if (name == null) Write(0); else SaveString(name);
            if (data == null) Write(0); else Save(data, index);
        }

        private void WriteDictNames(List<string> dictNames)
        {
            // Sort the dict ordinally.
            dictNames.Sort(ResStringComparer.Instance);

            // Write header.
            Write(sizeof(uint) * 2 + (dictNames.Count + 1) * _dictNodeSize);
            Write(dictNames.Count);

            // Write nodes.
            WriteDictNamesNode(UInt32.MaxValue, 1, 0, null); // Root node.
            foreach (string entry in dictNames)
            {
                WriteDictNamesNode(0x55555555, 0x6666, 0x7777, entry); // TODO: Compute node values.
            }
        }

        private void WriteDictNamesNode(uint reference, ushort idxLeft, ushort idxRight, string name)
        {
            Write(reference);
            Write(idxLeft);
            Write(idxRight);
            if (name == null) Write(0); else SaveString(name);
            if (name == null) Write(0); else SaveString(name);
        }

        private void WriteStrings()
        {
            // Sort the strings ordinally.
            SortedList<string, StringEntry> sorted = new SortedList<string, StringEntry>(ResStringComparer.Instance);
            foreach (KeyValuePair<string, StringEntry> entry in _savedStrings)
            {
                sorted.Add(entry.Key, entry.Value);
            }

            Align(4);
            uint stringPoolOffset = (uint)Position;

            foreach (KeyValuePair<string, StringEntry> entry in _savedStrings)
            {
                // Align and satisfy offsets.
                Write(entry.Key.Length);
                using (TemporarySeek())
                {
                    SatisfyOffsets(entry.Value.Offsets, (uint)Position);
                }

                // Write the name.
                Write(entry.Key, BinaryStringFormat.ZeroTerminated, entry.Value.Encoding ?? Encoding);
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
                if (entry.Value.Alignment != 0) Align((int)entry.Value.Alignment);
                using (TemporarySeek())
                {
                    SatisfyOffsets(entry.Value.Offsets, (uint)Position);
                }

                // Write the data.
                entry.Value.Callback.Invoke();
            }
        }

        private void WriteOffsets()
        {
            using (TemporarySeek())
            {
                foreach (KeyValuePair<object, ItemEntry> entry in _savedItems)
                {
                    SatisfyOffsets(entry.Value.Offsets, entry.Value.Target);
                }
            }
        }

        private void SatisfyOffsets(IEnumerable<uint> offsets, uint target)
        {
            foreach (uint offset in offsets)
            {
                Position = offset;
                Write((int)(target - offset));
            }
        }

        // ---- STRUCTURES ---------------------------------------------------------------------------------------------

        private enum ItemEntryType
        {
            List, Dict, DictList, DictNames, ResData, Data
        }

        private class ItemEntry
        {
            internal ItemEntryType Type;
            internal List<uint> Offsets;
            internal int Index;
            internal Action Callback;
            internal uint Target;

            internal ItemEntry(ItemEntryType type)
            {
                Type = type;
                Offsets = new List<uint>();
                Index = -1;
                Target = UInt32.MaxValue; // Recognize unsatisfied offsets easier.
            }
            
            internal ItemEntry(uint offset, ItemEntryType type, int index = -1, Action callback = null)
                : this(type)
            {
                Offsets.Add(offset);
                Index = index; // Remember for some element classes storing it.
                Callback = callback;
            }
        }

        private class StringEntry
        {
            internal List<uint> Offsets;
            internal Encoding Encoding;

            internal StringEntry(uint offset, Encoding encoding = null)
            {
                Offsets = new List<uint>(new uint[] { offset });
                Encoding = encoding;
            }
        }

        private class BlockEntry
        {
            internal List<uint> Offsets;
            internal uint Alignment;
            internal Action Callback;

            internal BlockEntry(uint offset, uint alignment, Action callback)
            {
                Offsets = new List<uint>(new uint[] { offset });
                Alignment = alignment;
                Callback = callback;
            }
        }
    }
}
