using Ifpen.AllotropeConverters.Chromeleon.Abstractions;

namespace Ifpen.AllotropeConverters.Chromeleon.PeakNameStrategies
{
    /// <summary>
    /// Default (passthrough) strategy that returns the original peak name unchanged.
    /// Used when no mapping strategy is configured.
    /// </summary>
    public class DefaultPeakNameStrategy : IPeakNameMappingStrategy
    {
        /// <inheritdoc />
        public string MapName(string originalName)
        {
            return originalName;
        }
    }
}
