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
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public ShaderParamType Type { get; set; }

        public ushort DataOffset { get; set; }

        public ushort DependedIndex { get; set; }

        public ushort DependIndex { get; set; }

        public string Name { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ShaderParamHead head = new ShaderParamHead(loader);
            Type = head.Type;
            DataOffset = head.OfsData;
            DependedIndex = head.IdxDepended;
            DependIndex = head.IdxDepend;
            Name = loader.GetName(head.OfsName);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="ShaderParam"/> instance.
    /// </summary>
    internal class ShaderParamHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal ShaderParamType Type;
        internal byte SizData;
        internal ushort OfsData;
        internal int Offset;
        internal uint CallbackPointer;
        internal ushort IdxDepended;
        internal ushort IdxDepend;
        internal uint OfsName;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ShaderParamHead(ResFileLoader loader)
        {
            // TODO: This layout does not work out for 3.2.0.1 files (s. MK8 Gear.bfres).
            if (loader.ResFile.Version >= 0x03030000)
            {
                Type = loader.ReadEnum<ShaderParamType>(true);
                SizData = loader.ReadByte();
                OfsData = loader.ReadUInt16();
                Offset = loader.ReadInt32();
                CallbackPointer = loader.ReadUInt32();
                IdxDepended = loader.ReadUInt16();
                IdxDepend = loader.ReadUInt16();
                OfsName = loader.ReadOffset();
            }
            else
            {
                // Guess
                Type = loader.ReadEnum<ShaderParamType>(true);
                loader.Seek(1);
                OfsData = loader.ReadUInt16();
                Offset = loader.ReadInt32();
                OfsName = loader.ReadOffset();
            }
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
        Srt2D, Srt3D, TexSrt, Matrix3x2 /*guess*/
    }
}