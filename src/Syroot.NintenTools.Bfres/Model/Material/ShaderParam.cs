using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a parameter value in a <see cref="UserData"/> section, passing data to shader variables.
    /// </summary>
    [DebuggerDisplay(nameof(ShaderParam) + " {" + nameof(Name) + "}")]
    public class ShaderParam : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public ShaderParamType Type { get; set; }

        public ushort DataOffset { get; set; }

        public ushort DependedIndex { get; set; }

        public ushort DependIndex { get; set; }

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{ShaderParam}"/> instances.
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

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            if (loader.ResFile.Version >= 0x03030000)
            {
                Type = loader.ReadEnum<ShaderParamType>(true);
                byte sizData = loader.ReadByte();
                DataOffset = loader.ReadUInt16();
                int offset = loader.ReadInt32(); // Uniform variable offset.
                uint callbackPointer = loader.ReadUInt32();
                DependedIndex = loader.ReadUInt16();
                DependIndex = loader.ReadUInt16();
                Name = loader.LoadString();
            }
            else
            {
                // GUESS
                Type = loader.ReadEnum<ShaderParamType>(true);
                loader.Seek(1);
                DataOffset = loader.ReadUInt16();
                int offset = loader.ReadInt32(); // Uniform variable offset.
                Name = loader.LoadString();
            }
        }
        
        void IResData.Save(ResFileSaver saver)
        {
        }
    }

    public enum ShaderParamType : byte
    {
        Bool, Bool2, Bool3, Bool4,
        Int, Int2, Int3, Int4,
        UInt, UInt2, UInt3, UInt4,
        Float, Float2, Float3, Float4,
        Float2x2 = 17, Float2x3, Float2x4,
        Float3x2 = 21, Float3x3, Float3x4,
        Float4x2 = 25, Float4x3, Float4x4,
        Srt2D, Srt3D, TexSrt, Matrix3x2/*GUESS*/
    }
}