using System;
using System.IO;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Ifpen.AllotropeConverters.Chromeleon.PeakNameStrategies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Ifpen.AllotropeConverters.Chromeleon.Configuration
{
    /// <summary>
    /// Factory that creates the appropriate <see cref="IPeakNameMappingStrategy"/> 
    /// based on a <see cref="PeakNameMappingConfig"/> or a JSON configuration file.
    /// Builds a decorator chain: core strategy → IFPEN cache → memory cache.
    /// </summary>
    public static class PeakNameStrategyFactory
    {
        private const string DefaultConfigFileName = "peakname-config.json";

        /// <summary>
        /// Creates a peak name mapping strategy from the default configuration file.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>A peak name mapping strategy.</returns>
        public static IPeakNameMappingStrategy CreateFromFile(ILoggerFactory loggerFactory)
        {
            return CreateFromFile(DefaultConfigFileName, loggerFactory);
        }


        /// <summary>
        /// Creates a peak name mapping strategy from a specified configuration file.
        /// </summary>
        /// <param name="configFilePath">The path to the JSON configuration file.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>A peak name mapping strategy.</returns>
        public static IPeakNameMappingStrategy CreateFromFile(string configFilePath, ILoggerFactory loggerFactory)

        {
            if (!File.Exists(configFilePath))
            {
                return new DefaultPeakNameStrategy();
            }

            try
            {
                string json = File.ReadAllText(configFilePath);
                var config = JsonConvert.DeserializeObject<PeakNameMappingConfig>(json);
                return Create(config, loggerFactory);
            }
            catch
            {
                // Fallback if config is invalid
                return new DefaultPeakNameStrategy();
            }
        }

        /// <summary>
        /// Creates a peak name mapping strategy from a configuration object.
        /// </summary>
        /// <param name="config">The mapping configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>A peak name mapping strategy.</returns>
        public static IPeakNameMappingStrategy Create(PeakNameMappingConfig config, ILoggerFactory loggerFactory)

        {
            if (config == null || !config.EnableWikidata)
            {
                return new DefaultPeakNameStrategy();
            }

            IPeakNameMappingStrategy strategy = new WikidataFrenchNameStrategy(
                 loggerFactory.CreateLogger<WikidataFrenchNameStrategy>()
            );

            // Wrap with memory cache if configured
            if (config.UseMemoryCache)
            {
                strategy = new MemoryCachePeakNameStrategy(strategy);
            }

            return strategy;
        }
    }
}
