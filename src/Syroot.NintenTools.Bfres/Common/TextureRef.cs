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

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{TextureRef}"/> instances. Typically the same as the <see cref="Texture.Name"/>.
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
            Name = loader.LoadString();
            Texture = loader.Load<Texture>();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.SaveString(Name);
            saver.Save(Texture);
        }
    }
}