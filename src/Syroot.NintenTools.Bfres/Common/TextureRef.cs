using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a reference to a <see cref="Texture"/> instance by name.
    /// </summary>
    [DebuggerDisplay(nameof(TextureRef) + " {" + nameof(Name) + "}")]
    public class TextureRef : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

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
            TextureRefHead head = new TextureRefHead(loader);
            Name = loader.GetName(head.OfsName);
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