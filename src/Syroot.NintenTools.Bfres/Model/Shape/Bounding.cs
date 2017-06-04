using System.Diagnostics;
using Syroot.Maths;

namespace Syroot.NintenTools.Bfres
{
    [DebuggerDisplay(nameof(Bounding) + " [{" + nameof(Center) + "},{" + nameof(Extent) + "})")]
    public struct Bounding
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public Vector3F Center;
        public Vector3F Extent;
    }
}
