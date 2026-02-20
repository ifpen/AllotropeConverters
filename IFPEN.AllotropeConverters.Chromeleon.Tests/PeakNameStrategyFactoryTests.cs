using FluentAssertions;
using Ifpen.AllotropeConverters.Chromeleon.Configuration;
using Ifpen.AllotropeConverters.Chromeleon.PeakNameStrategies;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests
{
    public class PeakNameStrategyFactoryTests
    {
        [Fact]
        public void Factory_ReturnsDefaultStrategy_WhenConfigIsNull()
        {
            var loggerFactory = new NullLoggerFactory();
            var strategy = PeakNameStrategyFactory.Create(null, loggerFactory);
            strategy.Should().BeOfType<DefaultPeakNameStrategy>();
        }

        [Fact]
        public void Factory_ReturnsDefaultStrategy_WhenWikidataIsDisabled()
        {
            var loggerFactory = new NullLoggerFactory();
            var config = new PeakNameMappingConfig { EnableWikidata = false };
            var strategy = PeakNameStrategyFactory.Create(config, loggerFactory);
            strategy.Should().BeOfType<DefaultPeakNameStrategy>();
        }

        [Fact]
        public void Factory_ReturnsWikidataStrategy_WhenEnabled()
        {
            var loggerFactory = new NullLoggerFactory();
            var config = new PeakNameMappingConfig { EnableWikidata = true };
            var strategy = PeakNameStrategyFactory.Create(config, loggerFactory);
            strategy.Should().BeOfType<WikidataFrenchNameStrategy>();
        }

        [Fact]
        public void Factory_ReturnsDefaultStrategyWithoutCache_WhenWikidataIsDisabledButCacheIsEnabled()
        {
            var loggerFactory = new NullLoggerFactory();
            var config = new PeakNameMappingConfig { EnableWikidata = false, UseMemoryCache = true };
            var strategy = PeakNameStrategyFactory.Create(config, loggerFactory);
            
            // Should be DefaultPeakNameStrategy, not wrapped in MemoryCachePeakNameStrategy
            strategy.Should().BeOfType<DefaultPeakNameStrategy>();
        }
    }
}
