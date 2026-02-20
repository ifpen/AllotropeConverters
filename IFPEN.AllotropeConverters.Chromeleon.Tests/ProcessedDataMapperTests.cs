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
        public void Map_WhenNoPeaksFound_ReturnsDocumentWithEmptyPeakList()
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

            // Assert - Per Allotrope schema, must return one document with empty peak list
            result.Should().HaveCount(1);
            result[0].PeakList.Should().NotBeNull();
            result[0].PeakList.Peak.Should().BeEmpty();
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
                ResolutionBaseline = 2.5,
                ResolutionHalfHeight = 2.0,
                ResolutionStatisticalMoments = 1.8,
                TheoreticalPlatesTangent = 5000,
                TheoreticalPlatesHalfHeight = 4500,
                Asymmetry = 1.1,
                CapacityFactor = 3.5,
                RelativeHeight = 15.0,
                RelativeArea = 12.0,
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
            peak.ChromatographicPeakResolutionUsingPeakWidthAtHalfHeight.Value.Should().Be(2.0);
            peak.ChromatographicPeakResolutionUsingStatisticalMoments.Value.Should().Be(1.8);
            peak.NumberOfTheoreticalPlatesByTangentMethod.Value.Should().Be(5000);
            peak.NumberOfTheoreticalPlatesByPeakWidthAtHalfHeight.Value.Should().Be(4500);
            peak.AsymmetryFactorMeasuredAt5Height.Value.Should().Be(1.1);
            peak.CapacityFactorChromatography.Value.Should().Be(3.5);
            peak.RelativePeakHeight.Value.Should().Be(15.0);
            peak.RelativePeakArea.Value.Should().Be(12.0);
            peak.StatisticalSkew.Value.Should().Be(0.5);
        }

        [Fact]
        public void Map_WhenPeakHasNullOptionalValues_ShouldNotThrowException()
        {
            // Arrange
            var injectionMock = new Mock<IInjection>();
            var signalMock = new Mock<ISignal>();
            signalMock.Setup(s => s.Injection).Returns(injectionMock.Object);
            signalMock.Setup(s => s.Name).Returns("UV_VIS_1");
            signalMock.Setup(s => s.Metadata.SignalAxis.Unit).Returns("mAU");

            // Create a peak with null optional statistical values (edge case from Issue #4)
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
                ResolutionBaseline = null,  // NULL - cannot be computed
                ResolutionHalfHeight = null,
                ResolutionStatisticalMoments = null,
                TheoreticalPlatesTangent = null,   // NULL - cannot be computed
                TheoreticalPlatesHalfHeight = null,
                Asymmetry = null,            // NULL - cannot be computed
                CapacityFactor = null,
                RelativeHeight = null,
                RelativeArea = null,
                Skewness = null              // NULL - cannot be computed
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
            peak.PeakHeight.Value.Should().Be(50.0);

            // These optional properties should be null when the source value is null
            peak.ChromatographicPeakResolutionUsingBaselinePeakWidths.Should().BeNull();
            peak.ChromatographicPeakResolutionUsingPeakWidthAtHalfHeight.Should().BeNull();
            peak.ChromatographicPeakResolutionUsingStatisticalMoments.Should().BeNull();
            peak.NumberOfTheoreticalPlatesByTangentMethod.Should().BeNull();
            peak.NumberOfTheoreticalPlatesByPeakWidthAtHalfHeight.Should().BeNull();
            peak.AsymmetryFactorMeasuredAt5Height.Should().BeNull();
            peak.CapacityFactorChromatography.Should().BeNull();
            peak.RelativePeakHeight.Should().BeNull();
            peak.RelativePeakArea.Should().BeNull();
            peak.StatisticalSkew.Should().BeNull();
        }

        [Fact]
        public void Map_WhenPeaksFound_ShouldMapCustomWidths()
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
                RetentionTimeSeconds = 10,
                StartTimeSeconds = 9,
                EndTimeSeconds = 11,
                Area = 100,
                Height = 10,
                WidthBaselineSeconds = 1.0,
                WidthHalfHeightSeconds = 0.5,
                Width5PercentHeightSeconds = 0.8
            };

            _peakProviderMock.Setup(p => p.GetPeaks(injectionMock.Object, "UV_VIS_1"))
                .Returns(new List<PeakData> { peakData });

            // Act
            var result = _mapper.Map(signalMock.Object);

            // Assert
            var peak = result[0].PeakList.Peak[0];
            peak.PeakWidthAtBaseline.Value.Should().Be(1.0);
            peak.PeakWidthAtHalfHeight.Value.Should().Be(0.5);
            peak.PeakWidthAt5OfHeight.Value.Should().Be(0.8);
        }

        [Fact]
        public void Map_WhenPeakHasNullBaselineWidth_ShouldNotThrow()
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
                RetentionTimeSeconds = 10,
                StartTimeSeconds = 9,
                EndTimeSeconds = 11,
                Area = 100,
                Height = 10,
                WidthBaselineSeconds = null // NULL baseline width (Issue #6)
            };

            _peakProviderMock.Setup(p => p.GetPeaks(injectionMock.Object, "UV_VIS_1"))
                .Returns(new List<PeakData> { peakData });

            // Act
            System.Action act = () => _mapper.Map(signalMock.Object);

            // Assert
            act.Should().NotThrow<System.InvalidOperationException>();
            var result = _mapper.Map(signalMock.Object);
            result[0].PeakList.Peak[0].PeakWidthAtBaseline.Should().BeNull();
        }

        [Fact]
        public void Map_WhenStrategyConfigured_UsesStrategyForPeakName()
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
                Name = "Acetone",
                RetentionTimeSeconds = 10,
                StartTimeSeconds = 9,
                EndTimeSeconds = 11,
                Area = 100,
                Height = 10
            };

            _peakProviderMock.Setup(p => p.GetPeaks(injectionMock.Object, "UV_VIS_1"))
                .Returns(new List<PeakData> { peakData });

            // Create a mock strategy that translates "Acetone" to "Acétone"
            var strategyMock = new Mock<Ifpen.AllotropeConverters.Chromeleon.Abstractions.IPeakNameMappingStrategy>();
            strategyMock.Setup(s => s.MapName("Acetone")).Returns("Acétone");

            var mapperWithStrategy = new ProcessedDataMapper(_peakProviderMock.Object, strategyMock.Object);

            // Act
            var result = mapperWithStrategy.Map(signalMock.Object);

            // Assert
            result[0].PeakList.Peak[0].WrittenName.Should().Be("Acétone");
            strategyMock.Verify(s => s.MapName("Acetone"), Times.Once);
        }
    }
}

