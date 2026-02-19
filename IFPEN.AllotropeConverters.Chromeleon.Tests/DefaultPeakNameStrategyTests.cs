using FluentAssertions;
using Ifpen.AllotropeConverters.Chromeleon.PeakNameStrategies;
using Xunit;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests
{
    public class DefaultPeakNameStrategyTests
    {
        [Fact]
        public void DefaultStrategy_ReturnsOriginalName()
        {
            var strategy = new DefaultPeakNameStrategy();
            strategy.MapName("Acetone").Should().Be("Acetone");
        }

        [Fact]
        public void DefaultStrategy_ReturnsNull_WhenInputIsNull()
        {
            var strategy = new DefaultPeakNameStrategy();
            strategy.MapName(null).Should().BeNull();
        }

        [Fact]
        public void DefaultStrategy_ReturnsEmpty_WhenInputIsEmpty()
        {
            var strategy = new DefaultPeakNameStrategy();
            strategy.MapName("").Should().BeEmpty();
        }
    }
}
