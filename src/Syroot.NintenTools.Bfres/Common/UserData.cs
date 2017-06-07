using System;
using System.Diagnostics;
using System.Text;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents custom user variables which can be attached to many sections and subfiles of a <see cref="ResFile"/>.
    /// </summary>
    [DebuggerDisplay(nameof(UserData) + " {" + nameof(Name) + "}")]
    public class UserData : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;
        private object _value;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_name != value)
                {
                    _name = value;
                    NameChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public UserDataType Type { get; private set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public int[] GetValueInt32Array()
        {
            return (int[])_value;
        }

        public float[] GetValueSingleArray()
        {
            return (float[])_value;
        }

        public string[] GetValueStringArray()
        {
            return (string[])_value;
        }

        public byte[] GetValueByteArray()
        {
            return (byte[])_value;
        }

        public void SetValue(int[] value)
        {
            _value = value;
        }

        public void SetValue(float[] value)
        {
            _value = value;
        }

        public void SetValue(string[] value, bool asUnicode = false)
        {
            Type = asUnicode ? UserDataType.WString : UserDataType.String;
            _value = value;
        }

        public void SetValue(byte[] value)
        {
            _value = value;
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            UserDataHead head = new UserDataHead(loader);
            Name = loader.GetName(head.OfsName);
            Type = head.Type;
            _value = head.Value;
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="UserData"/> instance.
    /// </summary>
    internal class UserDataHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint OfsName;
        internal ushort Count;
        internal UserDataType Type;
        internal object Value;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal UserDataHead(ResFileLoader loader)
        {
            OfsName = loader.ReadOffset();
            Count = loader.ReadUInt16();
            Type = loader.ReadEnum<UserDataType>(true);
            loader.Seek(1);
            switch (Type)
            {
                case UserDataType.Int32:
                    Value = loader.ReadInt32s(Count);
                    break;
                case UserDataType.Single:
                    Value = loader.ReadSingles(Count);
                    break;
                case UserDataType.String:
                    Value = loader.GetNames(loader.ReadOffsets(Count), Encoding.ASCII);
                    break;
                case UserDataType.WString:
                    Value = loader.GetNames(loader.ReadOffsets(Count), Encoding.Unicode);
                    break;
                case UserDataType.Bytes:
                    Value = loader.ReadBytes(Count);
                    break;
            }
        }
    }

    public enum UserDataType : byte
    {
        Int32,
        Single,
        String,
        WString,
        Bytes
    }
}