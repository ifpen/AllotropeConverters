namespace Ifpen.AllotropeConverters.Chromeleon.Abstractions
{
    /// <summary>
    /// Defines a strategy for mapping peak names to display names.
    /// Implementations can translate names, look up synonyms, or apply caching.
    /// </summary>
    public interface IPeakNameMappingStrategy
    {
        /// <summary>
        /// Maps the original peak name to a display name.
        /// </summary>
        /// <param name="originalName">The original peak name from the chromatogram.</param>
        /// <returns>The mapped name, or the original name if no mapping is found.</returns>
        string MapName(string originalName);
    }
}
