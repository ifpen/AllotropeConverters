using System.Collections.Generic;
using FluentAssertions;
using IFPEN.AllotropeConverters.AllotropeModels;
using Ifpen.AllotropeConverters.Chromeleon.Mappers;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Ifpen.AllotropeConverters.Domain;
using Moq;
using Thermo.Chromeleon.Sdk.Interfaces.Data;
using Xunit;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests
{
    public class ProcessedDataMapperTests
    {
        private readonly Mock<IPeakDataProvider> _peakProviderMock;
        private readonly ProcessedDataMapper _mapper;

        public ProcessedDataMapperTests()
        {
            _peakProviderMock = new Mock<IPeakDataProvider>();
            _mapper = new ProcessedDataMapper(_peakProviderMock.Object);
        }

        [Fact]
        public void Map_WhenInjectionIsNull_ReturnsEmptyList()
        {
            // Arrange
            var signalMock = new Mock<ISignal>();
            signalMock.Setup(s => s.Injection).Returns((IInjection)null);

            // Act
            var result = _mapper.Map(signalMock.Object);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Map_WhenNoPeaksFound_ReturnsEmptyList()
        {
            // Arrange
            var injectionMock = new Mock<IInjection>();
            var signalMock = new Mock<ISignal>();
            signalMock.Setup(s => s.Injection).Returns(injectionMock.Object);
            signalMock.Setup(s => s.Name).Returns("UV_VIS_1");

            _peakProviderMock.Setup(p => p.GetPeaks(injectionMock.Object, "UV_VIS_1"))
                .Returns(new List<PeakData>());

            // Act
            var result = _mapper.Map(signalMock.Object);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Map_WhenPeaksFound_ReturnsMappedDocument()
        {
            // Arrange
            var injectionMock = new Mock<IInjection>();
            var signalMock = new Mock<ISignal>();
            signalMock.Setup(s => s.Injection).Returns(injectionMock.Object);
            signalMock.Setup(s => s.Name).Returns("UV_VIS_1");
            signalMock.Setup(s => s.Metadata.SignalAxis.Unit).Returns("mAU");

            var peakData = new PeakData
            {
                Index = 1,
                Name = "Peak 1",
                RetentionTimeSeconds = 12.5,
                StartTimeSeconds = 10.0,
                EndTimeSeconds = 15.0,
                Area = 1000.0,
                Height = 50.0,
                WidthBaselineSeconds = 5.0,
                Resolution = 2.5,
                TheoreticalPlates = 5000,
                Asymmetry = 1.1,
                Skewness = 0.5
            };

            _peakProviderMock.Setup(p => p.GetPeaks(injectionMock.Object, "UV_VIS_1"))
                .Returns(new List<PeakData> { peakData });

            // Act
            var result = _mapper.Map(signalMock.Object);

            // Assert
            result.Should().HaveCount(1);
            var peakList = result[0].PeakList;
            peakList.Should().NotBeNull();
            peakList.Peak.Should().HaveCount(1);

            var peak = peakList.Peak[0];
            peak.Identifier.Should().Be("1");
            peak.WrittenName.Should().Be("Peak 1");
            peak.RetentionTime.Value.Should().Be(12.5);
            peak.PeakArea.Value.Should().Be(1000.0);
            peak.PeakArea.Unit.Should().Be("mAU s");
            peak.PeakHeight.Value.Should().Be(50.0);
            peak.PeakHeight.Unit.Should().Be("mAU");
            
            // Validate stats
            peak.ChromatographicPeakResolutionUsingBaselinePeakWidths.Value.Should().Be(2.5);
            peak.NumberOfTheoreticalPlatesByTangentMethod.Value.Should().Be(5000);
            peak.AsymmetryFactorMeasuredAt5Height.Value.Should().Be(1.1);
            peak.StatisticalSkew.Value.Should().Be(0.5);
        }
    }
}
