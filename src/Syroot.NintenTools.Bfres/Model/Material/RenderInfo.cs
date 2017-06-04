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
    public class RenderInfo : IResContent
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private object _value;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public RenderInfoType Type { get; private set; }

        public string Name { get; set; }
        
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public void Load(ResFileLoader loader)
        {
            RenderInfoHead head = new RenderInfoHead(loader);
            Type = head.Type;
            Name = loader.GetName(head.OfsName);
            _value = head.Value;
        }

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

        public void SetValue(int[] value)
        {
            _value = value;
        }

        public void SetValue(float[] value)
        {
            _value = value;
        }

        public void SetValue(string[] value)
        {
            _value = value;
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