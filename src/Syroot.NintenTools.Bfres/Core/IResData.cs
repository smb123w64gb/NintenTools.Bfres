namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Represents the common interface for <see cref="ResFile"/> instances.
    /// </summary>
    public interface IResData
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Loads raw data from the <paramref name="loader"/> data stream into instances.
        /// </summary>
        /// <param name="loader">The <see cref="ResFileLoader"/> to load data with.</param>
        void Load(ResFileLoader loader);

        /// <summary>
        /// Resolves any references between <see cref="IResData"/> instances.
        /// </summary>
        /// <param name="resFileLoader">The <see cref="ResFileLoader"/> to query reference instances with.</param>
        void Reference(ResFileLoader resFileLoader);
    }
}
