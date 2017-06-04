using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;
using Syroot.NintenTools.Bfres.GX2;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a <see cref="Texture"/> sampler in a <see cref="Material"/> section, storing configuration on how to
    /// draw and interpolate textures.
    /// </summary>
    [DebuggerDisplay(nameof(Sampler) + " {" + nameof(Name) + "}")]
    public class Sampler : IResContent
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public TexSampler TexSampler { get; set; }

        public string Name { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public void Load(ResFileLoader loader)
        {
            SamplerHead head = new SamplerHead(loader);
            TexSampler = new TexSampler(head.GX2Sampler);
            Name = loader.GetName(head.OfsName);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="Sampler"/> instance.
    /// </summary>
    internal class SamplerHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public uint[] GX2Sampler;
        public uint Handle;
        public uint OfsName;
        public byte Idx;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal SamplerHead(ResFileLoader loader)
        {
            GX2Sampler = loader.ReadUInt32s(3);
            Handle = loader.ReadUInt32();
            OfsName = loader.ReadOffset();
            Idx = loader.ReadByte();
            loader.Seek(3);
        }
    }
}