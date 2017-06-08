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
    public class TextureRef : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;
        private uint _ofsTexture;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        /// <summary>
        /// The name with which the instance can be referenced uniquely in <see cref="INamedResDataList{TextureRef}"/>
        /// instances. Typically the same as the <see cref="Texture.Name"/>.
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

        /// <summary>
        /// The referenced <see cref="Texture"/> instance.
        /// </summary>
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