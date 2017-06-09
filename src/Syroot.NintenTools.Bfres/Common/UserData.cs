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

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{UserData}"/> instances.
        /// </summary>
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

        /// <summary>
        /// The data type of the stored values.
        /// </summary>
        public UserDataType Type { get; private set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Returns the stored value as an array of <see cref="Int32"/> instances when the <see cref="Type"/> is
        /// <see cref="UserDataType.Int32"/>.
        /// </summary>
        /// <returns>The typed value.</returns>
        public int[] GetValueInt32Array()
        {
            return (int[])_value;
        }

        /// <summary>
        /// Returns the stored value as an array of <see cref="Single"/> instances when the <see cref="Type"/> is
        /// <see cref="UserDataType.Single"/>.
        /// </summary>
        /// <returns>The typed value.</returns>
        public float[] GetValueSingleArray()
        {
            return (float[])_value;
        }

        /// <summary>
        /// Returns the stored value as an array of <see cref="String"/> instances when the <see cref="Type"/> is
        /// <see cref="UserDataType.String"/> or <see cref="UserDataType.WString"/>.
        /// </summary>
        /// <returns>The typed value.</returns>
        public string[] GetValueStringArray()
        {
            return (string[])_value;
        }

        /// <summary>
        /// Returns the stored value as an array of <see cref="Byte"/> instances when the <see cref="Type"/> is
        /// <see cref="UserDataType.Byte"/>.
        /// </summary>
        /// <returns>The typed value.</returns>
        public byte[] GetValueByteArray()
        {
            return (byte[])_value;
        }

        /// <summary>
        /// Sets the stored <paramref name="value"/> as an <see cref="Int32"/> array and the <see cref="Type"/> to
        /// <see cref="UserDataType.Int32"/>
        /// </summary>
        /// <param name="value">The value to store.</param>
        public void SetValue(int[] value)
        {
            _value = value;
        }

        /// <summary>
        /// Sets the stored <paramref name="value"/> as a <see cref="Single"/> array and the <see cref="Type"/> to
        /// <see cref="UserDataType.Single"/>
        /// </summary>
        /// <param name="value">The value to store.</param>
        public void SetValue(float[] value)
        {
            _value = value;
        }

        /// <summary>
        /// Sets the stored <paramref name="value"/> as a <see cref="String"/> array and the <see cref="Type"/> to
        /// <see cref="UserDataType.String"/> or <see cref="UserDataType.WString"/> depending on
        /// <paramref name="asUnicode"/>.
        /// </summary>
        /// <param name="asUnicode"><c>true</c> to store data as UTF-16 encoded strings, or <c>false</c> to store it
        /// as ASCII encoded strings.</param>
        /// <param name="value">The value to store.</param>
        public void SetValue(string[] value, bool asUnicode = false)
        {
            Type = asUnicode ? UserDataType.WString : UserDataType.String;
            _value = value;
        }

        /// <summary>
        /// Sets the stored <paramref name="value"/> as a <see cref="Byte"/> array and the <see cref="Type"/> to
        /// <see cref="UserDataType.Byte"/>
        /// </summary>
        /// <param name="value">The value to store.</param>
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
                case UserDataType.Byte:
                    Value = loader.ReadBytes(Count);
                    break;
            }
        }
    }

    /// <summary>
    /// Represents the possible data types of values stored in <see cref="UserData"/> instances.
    /// </summary>
    public enum UserDataType : byte
    {
        /// <summary>
        /// The values is an <see cref="Int32"/> array.
        /// </summary>
        Int32,

        /// <summary>
        /// The values is a <see cref="Single"/> array.
        /// </summary>
        Single,

        /// <summary>
        /// The values is a <see cref="String"/> array encoded in ASCII.
        /// </summary>
        String,

        /// <summary>
        /// The values is a <see cref="String"/> array encoded in UTF-16.
        /// </summary>
        WString,

        /// <summary>
        /// The values is a <see cref="Byte"/> array.
        /// </summary>
        Byte
    }
}