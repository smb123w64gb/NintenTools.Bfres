using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a render info in a FMAT section storing uniform parameters required to render the
    /// <see cref="UserData"/>.
    /// </summary>
    [DebuggerDisplay(nameof(RenderInfo) + " {" + nameof(Name) + "}")]
    public class RenderInfo : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;
        private object _value;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        public event EventHandler NameChanged;
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public RenderInfoType Type { get; private set; }

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

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public int[] GetValueInt32s()
        {
            return (int[])_value;
        }

        public float[] GetValueSingles()
        {
            return (float[])_value;
        }

        public string[] GetValueStrings()
        {
            return (string[])_value;
        }

        public void SetValue(int[] value)
        {
            Type = RenderInfoType.Int32;
            _value = value;
        }

        public void SetValue(float[] value)
        {
            Type = RenderInfoType.Single;
            _value = value;
        }

        public void SetValue(string[] value)
        {
            Type = RenderInfoType.String;
            _value = value;
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            RenderInfoHead head = new RenderInfoHead(loader);
            Type = head.Type;
            Name = loader.GetName(head.OfsName);
            _value = head.Value;
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="RenderInfo"/> instance.
    /// </summary>
    internal class RenderInfoHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal ushort SizArray;
        internal RenderInfoType Type;
        internal uint OfsName;
        internal object Value;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal RenderInfoHead(ResFileLoader loader)
        {
            SizArray = loader.ReadUInt16();
            Type = loader.ReadEnum<RenderInfoType>(true);
            loader.Seek(1);
            OfsName = loader.ReadOffset();
            switch (Type)
            {
                case RenderInfoType.Int32:
                    Value = loader.ReadInt32s(SizArray);
                    break;
                case RenderInfoType.Single:
                    Value = loader.ReadSingles(SizArray);
                    break;
                case RenderInfoType.String:
                    Value = loader.ReadUInt32s(SizArray);
                    break;
            }
        }
    }

    public enum RenderInfoType : byte
    {
        Int32,
        Single,
        String
    }
}