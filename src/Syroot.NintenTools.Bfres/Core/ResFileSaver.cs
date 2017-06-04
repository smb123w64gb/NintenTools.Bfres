using System.IO;
using Syroot.BinaryData;
using Syroot.Maths;
using System.Text;

namespace Syroot.NintenTools.Bfres.Core
{
    internal class ResFileSaver : BinaryDataWriter
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ResFileSaver(ResFile resFile, Stream stream, bool leaveOpen)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
            ByteOrder = ByteOrder.BigEndian;
        }

        internal ResFileSaver(ResFile resFile, string fileName)
            : this(resFile, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), false)
        {
        }
        
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Writes a <see cref="Vector3"/> instance into the current stream.
        /// </summary>
        /// <param name="value">The <see cref="Vector3"/> instance.</param>
        internal void Write(Vector3 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }

        /// <summary>
        /// Writes <see cref="Vector3"/> instances into the current stream.
        /// </summary>
        /// <param name="values">The <see cref="Vector3"/> instances.</param>
        internal void Write(Vector3[] values)
        {
            foreach (Vector3 value in values)
            {
                Write(value);
            }
        }

        /// <summary>
        /// Writes a <see cref="Vector3F"/> instance into the current stream.
        /// </summary>
        /// <param name="value">The <see cref="Vector3F"/> instance.</param>
        internal void Write(Vector3F value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }

        /// <summary>
        /// Writes <see cref="Vector3F"/> instances into the current stream.
        /// </summary>
        /// <param name="values">The <see cref="Vector3F"/> instances.</param>
        internal void Write(Vector3F[] values)
        {
            foreach (Vector3F value in values)
            {
                Write(value);
            }
        }
    }
}
