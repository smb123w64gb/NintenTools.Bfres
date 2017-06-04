using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a subarray of a <see cref="Mesh"/> section, storing a slice of indices to draw from the index buffer
    /// referenced in the mesh, mostly used for hiding parts of a model when not visible.
    /// </summary>
    [DebuggerDisplay(nameof(SubMesh) + " [{" + nameof(Offset) + "},{" + nameof(Count) + "})")]
    public class SubMesh : IResContent
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public uint Offset { get; set; }

        public uint Count { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public void Load(ResFileLoader loader)
        {
            Offset = loader.ReadUInt32();
            Count = loader.ReadUInt32();
        }
    }
}