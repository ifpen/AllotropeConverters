using System.Net.Http;
using FluentAssertions;
using Ifpen.AllotropeConverters.Chromeleon.PeakNameStrategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests
{
    public class WikidataIntegrationTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void GetFrenchName_Live_EnanthylicAcid_ReturnsAcideHeptanoique()
        {
            // Arrange
            // Use NullLogger for integration verification.
            var logger = NullLogger<WikidataFrenchNameStrategy>.Instance;
            var httpClient = new HttpClient();
            var strategy = new WikidataFrenchNameStrategy(logger, httpClient);

            // Act
            var result = strategy.MapName("Acide énanthique");

            // Assert
            result.Should().Be("acide heptanoïque");
        }
    }
}
