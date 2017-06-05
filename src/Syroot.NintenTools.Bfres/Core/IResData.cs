namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Represents the common interface for <see cref="ResFile"/> structures.
    /// </summary>
    public interface IResData
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        void Load(ResFileLoader loader);
    }
}
