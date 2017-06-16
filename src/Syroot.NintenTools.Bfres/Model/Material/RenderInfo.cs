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

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public RenderInfoType Type { get; private set; }

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{RenderInfo}"/> instances.
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
            ushort sizArray = loader.ReadUInt16();
            Type = loader.ReadEnum<RenderInfoType>(true);
            loader.Seek(1);
            Name = loader.LoadString();
            switch (Type)
            {
                case RenderInfoType.Int32:
                    _value = loader.ReadInt32s(sizArray);
                    break;
                case RenderInfoType.Single:
                    _value = loader.ReadSingles(sizArray);
                    break;
                case RenderInfoType.String:
                    _value = loader.ReadUInt32s(sizArray);
                    break;
            }
        }
        
        void IResData.Save(ResFileSaver saver)
        {
        }
    }

    public enum RenderInfoType : byte
    {
        Int32,
        Single,
        String
    }
}