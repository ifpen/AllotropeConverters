using System;
using FluentAssertions;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Ifpen.AllotropeConverters.Chromeleon.PeakNameStrategies;
using Moq;
using Xunit;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests
{
    public class MemoryCachePeakNameStrategyTests
    {
        [Fact]
        public void MemoryCache_DelegatesOnFirstCall()
        {
            var innerMock = new Mock<IPeakNameMappingStrategy>();
            innerMock.Setup(s => s.MapName("Acetone")).Returns("Acétone");

            var cache = new MemoryCachePeakNameStrategy(innerMock.Object);

            var result = cache.MapName("Acetone");

            result.Should().Be("Acétone");
            innerMock.Verify(s => s.MapName("Acetone"), Times.Once);
        }

        [Fact]
        public void MemoryCache_CachesResult_OnSecondCall()
        {
            var innerMock = new Mock<IPeakNameMappingStrategy>();
            innerMock.Setup(s => s.MapName("Acetone")).Returns("Acétone");

            var cache = new MemoryCachePeakNameStrategy(innerMock.Object);

            cache.MapName("Acetone");
            var result = cache.MapName("Acetone");

            result.Should().Be("Acétone");
            innerMock.Verify(s => s.MapName("Acetone"), Times.Once);
        }

        [Fact]
        public void MemoryCache_IsCaseInsensitive()
        {
            var innerMock = new Mock<IPeakNameMappingStrategy>();
            innerMock.Setup(s => s.MapName("acetone")).Returns("Acétone");

            var cache = new MemoryCachePeakNameStrategy(innerMock.Object);

            cache.MapName("acetone");
            var result = cache.MapName("ACETONE");

            result.Should().Be("Acétone");
            innerMock.Verify(s => s.MapName(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void MemoryCache_ThrowsOnNullInner()
        {
            Action act = () => new MemoryCachePeakNameStrategy(null);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
