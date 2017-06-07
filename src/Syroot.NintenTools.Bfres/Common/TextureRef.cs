using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    // TODO: This class should possibly not exist, reference textures directly.

    /// <summary>
    /// Represents a reference to a <see cref="Texture"/> instance by name.
    /// </summary>
    [DebuggerDisplay(nameof(TextureRef) + " {" + nameof(Name) + "}")]
    public class TextureRef : IResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private uint _ofsTexture;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string Name { get; set; }

        public Texture Texture { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            TextureRefHead head = new TextureRefHead(loader);
            Name = loader.GetName(head.OfsName);
            _ofsTexture = head.OfsTexture;
        }

        void IResData.Reference(ResFileLoader loader)
        {
            Texture = loader.GetData<Texture>(_ofsTexture);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="TextureRef"/> instance.
    /// </summary>
    internal class TextureRefHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------
        
        internal uint OfsName;
        internal uint OfsTexture;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal TextureRefHead(ResFileLoader loader)
        {
            OfsName = loader.ReadOffset();
            OfsTexture = loader.ReadOffset();
        }
    }
}