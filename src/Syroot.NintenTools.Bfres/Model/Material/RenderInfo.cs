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
    public class RenderInfo : IResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------
        
        private object _value;
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public RenderInfoType Type { get; private set; }

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="ResDict{RenderInfo}"/> instances.
        /// </summary>
        public string Name { get; set; }

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
            ushort lenArray = loader.ReadUInt16();
            Type = loader.ReadEnum<RenderInfoType>(true);
            loader.Seek(1);
            Name = loader.LoadString();
            switch (Type)
            {
                case RenderInfoType.Int32:
                    _value = loader.ReadInt32s(lenArray);
                    break;
                case RenderInfoType.Single:
                    _value = loader.ReadSingles(lenArray);
                    break;
                case RenderInfoType.String:
                    _value = loader.LoadStrings(lenArray);
                    break;
            }
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.Write((ushort)((Array)_value).Length); // TODO: Unsafe cast, but _value should always be Array.
            saver.Write(Type, true);
            saver.Seek(1);
            saver.SaveString(Name);
            switch (Type)
            {
                case RenderInfoType.Int32:
                    saver.Write((int[])_value);
                    saver.Write(0); // Weird padding for numerical values.
                    break;
                case RenderInfoType.Single:
                    saver.Write((float[])_value);
                    saver.Write(0); // Weird padding for numerical values.
                    break;
                case RenderInfoType.String:
                    saver.SaveStrings((string[])_value);
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