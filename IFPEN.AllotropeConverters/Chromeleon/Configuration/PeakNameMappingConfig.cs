using Newtonsoft.Json;

namespace Ifpen.AllotropeConverters.Chromeleon.Configuration
{
    /// <summary>
    /// Configuration for the peak name mapping strategy.
    /// Loaded from a JSON configuration file (peakname-config.json).
    /// </summary>
    public class PeakNameMappingConfig
    {
        /// <summary>
        /// Whether to enable Wikidata translation. Default is false (passthrough).
        /// </summary>
        [JsonProperty("enableWikidata")]
        public bool EnableWikidata { get; set; }

        /// <summary>
        /// Whether to wrap the strategy with an in-memory cache.
        /// </summary>
        [JsonProperty("useMemoryCache")]
        public bool UseMemoryCache { get; set; }
    }
}
