using System.Diagnostics;
using System.Text;
using Syroot.BinaryData;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a <see cref="String"/> which is stored in a <see cref="ResFile"/>.
    /// </summary>
    [DebuggerDisplay("{" + nameof(String) + "}")]
    public class ResString : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string String
        {
            get; set;
        }

        public Encoding Encoding
        {
            get; set;
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Converts the given <paramref name="value"/> value to a <see cref="ResString"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to represent in the new <see cref="ResString"/> instance.
        /// </param>
        public static implicit operator ResString(string value)
        {
            return new ResString() { String = value };
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> value to an <see cref="String"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="ResString"/> value to represent in the new <see cref="String"/> instance.
        /// </param>
        public static implicit operator string(ResString value)
        {
            return value.String;
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public override string ToString()
        {
            return String;
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            String = loader.ReadString(BinaryStringFormat.ZeroTerminated, Encoding ?? loader.Encoding);
        }

        void IResData.Save(ResFileSaver saver)
        {
            saver.SaveString(String, Encoding);
        }
    }
}
