using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a render info in a FMAT section storing uniform parameters required to render the
    /// <see cref="Material"/>.
    /// </summary>
    [DebuggerDisplay(nameof(RenderInfo) + " {" + nameof(Name) + "}")]
    public class RenderInfo : ResContent
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------
        
        private object _value;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public RenderInfo(ResFileLoader loader)
            : base(loader)
        {
            RenderInfoHead head = new RenderInfoHead(loader);
            Type = head.Type;
            Name = loader.GetName(head.OfsName);
            switch (Type)
            {
                case RenderInfoType.Int32:
                    _value = head.Int32Values;
                    break;
                case RenderInfoType.Single:
                    _value = head.SingleValues;
                    break;
                case RenderInfoType.String:
                    _value = loader.GetNames(head.OfsStringValues);
                    break;
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public RenderInfoType Type { get; private set; }

        public string Name { get; set; }

        public object Value
        {
            get { return _value; }
            set
            {
                switch (value)
                {
                    case int[] int32Values:
                        Type = RenderInfoType.Int32;
                        break;
                    case float[] singleValues:
                        Type = RenderInfoType.Single;
                        break;
                    case string[] stringValues:
                        Type = RenderInfoType.String;
                        break;
                    default:
                        throw new ArgumentException(
                            $"Type {_value.GetType()} cannot be used as a {nameof(RenderInfo)} value.");
                }
                _value = value;
            }
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
        internal int[] Int32Values;
        internal float[] SingleValues;
        internal uint[] OfsStringValues;

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
                    Int32Values = loader.ReadInt32s(SizArray);
                    break;
                case RenderInfoType.Single:
                    SingleValues = loader.ReadSingles(SizArray);
                    break;
                case RenderInfoType.String:
                    OfsStringValues = loader.ReadUInt32s(SizArray);
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