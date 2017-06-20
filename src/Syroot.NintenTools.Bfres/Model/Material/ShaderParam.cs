using System;
using System.Diagnostics;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a parameter value in a <see cref="UserData"/> section, passing data to shader variables.
    /// </summary>
    [DebuggerDisplay(nameof(ShaderParam) + " {" + nameof(Name) + "}")]
    public class ShaderParam : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        public ShaderParamType Type { get; set; }

        /// <summary>
        /// Gets the offset in the <see cref="Material.ParamData"/> byte array in bytes.
        /// </summary>
        public ushort DataOffset { get; set; }

        public ushort DependedIndex { get; set; }

        public ushort DependIndex { get; set; }

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="ResDict{ShaderParam}"/> instances.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the size of the value in bytes.
        /// </summary>
        public uint DataSize
        {
            get
            {
                if (Type <= ShaderParamType.Float4)
                {
                    return sizeof(float) * (((uint)Type & 0x03) + 1);
                }
                if (Type <= ShaderParamType.Float4x4)
                {
                    uint cols = ((uint)Type & 0x03) + 1;
                    uint rows = (((uint)Type - (uint)ShaderParamType.Reserved2) >> 2) + 2;
                    return sizeof(float) * cols * rows;
                }
                switch (Type)
                {
                    case ShaderParamType.Srt2D: return Srt2D.SizeInBytes;
                    case ShaderParamType.Srt3D: return Srt3D.SizeInBytes;
                    case ShaderParamType.TexSrt: return TexSrt.SizeInBytes;
                    case ShaderParamType.TexSrtEx: return TexSrtEx.SizeInBytes;
                }
                throw new ResException($"Cannot retrieve size of unknown {nameof(ShaderParamType)} {Type}.");
            }
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            Type = loader.ReadEnum<ShaderParamType>(true);
            if (loader.ResFile.Version >= 0x03030000)
            {
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
                loader.Seek(1);
                DataOffset = loader.ReadUInt16();
                int offset = loader.ReadInt32(); // Uniform variable offset.
                Name = loader.LoadString();
            }
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.Write(Type, true);
            if (saver.ResFile.Version >= 0x03030000)
            {
                saver.Write((byte)DataSize);
                saver.Write(DataOffset);
                saver.Write(-1); // Offset
                saver.Write(0); // CallbackPointer
                saver.Write(DependedIndex);
                saver.Write(DependIndex);
                saver.SaveString(Name);
            }
            else
            {
                saver.Seek(1);
                saver.Write(DataOffset);
                saver.Write(-1); // Offset
                saver.SaveString(Name);
            }
        }
    }

    public enum ShaderParamType : byte
    {
        Bool, Bool2, Bool3, Bool4,
        Int, Int2, Int3, Int4,
        UInt, UInt2, UInt3, UInt4,
        Float, Float2, Float3, Float4,
        Reserved2, Float2x2, Float2x3, Float2x4,
        Reserved3, Float3x2, Float3x3, Float3x4,
        Reserved4, Float4x2, Float4x3, Float4x4,
        Srt2D, Srt3D, TexSrt, TexSrtEx
    }
}