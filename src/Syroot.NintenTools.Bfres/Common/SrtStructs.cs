using Syroot.Maths;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a 2D transformation.
    /// </summary>
    public struct Srt2D
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        public const int SizeInBytes = Vector2F.SizeInBytes + sizeof(float) + Vector2F.SizeInBytes;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public Vector2F Scaling;
        public float Rotation;
        public Vector2F Translation;
    }

    /// <summary>
    /// Represents a 3D transformation.
    /// </summary>
    public struct Srt3D
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        public const int SizeInBytes = Vector3F.SizeInBytes + Vector3F.SizeInBytes + Vector3F.SizeInBytes;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public Vector3F Scaling;
        public Vector3F Rotation;
        public Vector3F Translation;
    }

    /// <summary>
    /// Represents a texture 3D transformation.
    /// </summary>
    public struct TexSrt
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        public const int SizeInBytes = sizeof(TexSrtMode) + Vector2F.SizeInBytes + sizeof(float) + Vector2F.SizeInBytes;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public TexSrtMode mode;
        public Vector2F Scaling;
        public float Rotation;
        public Vector2F Translation;
    }

    /// <summary>
    /// Represents a texture 3D transformation which is multiplied by a 3x4 matrix referenced at runtime by the
    /// <see cref="MatrixPointer"/>.
    /// </summary>
    public struct TexSrtEx
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        public const int SizeInBytes = TexSrt.SizeInBytes + sizeof(uint);

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public TexSrtMode mode;
        public Vector2F Scaling;
        public float Rotation;
        public Vector2F Translation;
        public uint MatrixPointer;
    }

    /// <summary>
    /// Represents the texture transformation mode used in <see cref="TexSrt"/> and <see cref="TexSrtEx"/>.
    /// </summary>
    public enum TexSrtMode : uint
    {
        ModeMaya,
        Mode3dsMax,
        ModeSoftimage
    }
}
