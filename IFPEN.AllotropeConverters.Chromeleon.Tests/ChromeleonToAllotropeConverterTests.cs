using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using IFPEN.AllotropeConverters.AllotropeModels;
using IFPEN.AllotropeConverters.Chromeleon.Tests.TestHelpers;
using Ifpen.AllotropeConverters.Chromeleon;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Moq;
using Thermo.Chromeleon.Sdk.Common;
using Thermo.Chromeleon.Sdk.Interfaces;
using Thermo.Chromeleon.Sdk.Interfaces.Data;
using Thermo.Chromeleon.Sdk.Interfaces.Instruments.Symbols;
using Xunit;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests
{
    public class ChromeleonToAllotropeConverterTests
    {
        private readonly Mock<IDeviceSystemMapper> _deviceMapperMock;
        private readonly Mock<IChromatographyColumnMapper> _columnMapperMock;
        private readonly Mock<ISampleMapper> _sampleMapperMock;
        private readonly Mock<IDataCubeMapper> _dataCubeMapperMock;
        private readonly Mock<IProcessedDataMapper> _processedDataMapperMock;
        private readonly ChromeleonToAllotropeConverter _converter;

        public ChromeleonToAllotropeConverterTests()
        {
            _deviceMapperMock = new Mock<IDeviceSystemMapper>();
            _columnMapperMock = new Mock<IChromatographyColumnMapper>();
            _sampleMapperMock = new Mock<ISampleMapper>();
            _dataCubeMapperMock = new Mock<IDataCubeMapper>();
            _processedDataMapperMock = new Mock<IProcessedDataMapper>();

            _converter = new ChromeleonToAllotropeConverter(
                _deviceMapperMock.Object,
                _columnMapperMock.Object,
                _sampleMapperMock.Object,
                _dataCubeMapperMock.Object,
                _processedDataMapperMock.Object
            );
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Convert_GivenNullInjection_ThrowsArgumentNullException()
        {
            IInjection injection = null;
            // Act
            Action act = () => _converter.Convert(injection);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Convert_GivenValidInjection_ReturnsModel()
        {
            // Arrange - Create mock signal using SignalBuilder
            var signal = new SignalBuilder()
                .WithId(Guid.NewGuid())
                .WithDetector("Detector1")
                .WithUser("Analyst1")
                .WithUnit("mAU")
                .Build();

            // Arrange - Create a mock root symbol
            var rootSymbolMock = new Mock<ISymbol>();
            rootSymbolMock.Setup(s => s.Name).Returns("RootSymbol");

            // Arrange - Create mock injection using InjectionBuilder
            var injection = new InjectionBuilder()
                .WithName("TestInjection")
                .WithInjectTime(new DateTimeOffset(2025, 6, 23, 10, 0, 0, TimeSpan.Zero))
                .WithInjectionVolume(10.0)
                .WithInstrumentMethodName("TestMethod")
                .WithSignals(signal)
                .WithRootSymbol(rootSymbolMock.Object)
                .Build();

            // Arrange - Mock mapper responses with valid required properties
            _deviceMapperMock.Setup(m => m.Map(It.IsAny<ISymbol>()))
                .Returns(new DeviceSystemDocument(
                    assetManagementIdentifier: "TestDevice",
                    deviceIdentifier: "Device-001",
                    modelNumber: null,
                    equipmentSerialNumber: "SN-12345",
                    firmwareVersion: null,
                    description: null,
                    brandName: "Thermo Scientific",
                    productManufacturer: "Thermo Scientific",
                    pumpModelNumber: null,
                    detectorModelNumber: null,
                    deviceDocument: null
                ));
            
            _dataCubeMapperMock.Setup(m => m.Map(It.IsAny<ISignal>()))
                .Returns(new ChromatogramDataCube(
                    label: "Mock Signal",
                    datacubeStructure: new ChromatogramDatacubeStructure(
                        dimensions: new List<ChromatogramDimension>(),
                        measures: new List<ChromatogramMeasure>()
                    ),
                    datacubeData: new DatacubeData(
                        dimensions: new List<List<double>>(),
                        measures: new List<List<double>>()
                    )
                ));
            
            _columnMapperMock.Setup(m => m.Map(It.IsAny<ISymbol>()))
                .Returns(new ChromatographyColumnDocument(
                    chromatographyColumnSerialNumber: "Col-123",
                    chromatographyColumnChemistryType: "Test Column",
                    productManufacturer: "Thermo",
                    chromatographyColumnLength: null,
                    columnInnerDiameter: null,
                    chromatographyColumnParticleSize: null
                ));
            
            _sampleMapperMock.Setup(m => m.Map(It.IsAny<IInjection>()))
                .Returns(new SampleDocument(
                    sampleIdentifier: "Sample-001",
                    description: null,
                    writtenName: "Sample-001",
                    batchIdentifier: "N/A"
                ));
            
            _processedDataMapperMock.Setup(m => m.Map(It.IsAny<ISignal>()))
                .Returns(new List<ProcessedDataDocument>());

            // Act
            var result = _converter.Convert(injection);

            // Assert
            result.Should().NotBeNull();
            result.GasChromatographyAggregateDocument.Should().NotBeNull();
            result.GasChromatographyAggregateDocument.GasChromatographyDocument.Should().HaveCount(1);
            
            var doc = result.GasChromatographyAggregateDocument.GasChromatographyDocument[0];
            doc.Analyst.Should().Be("Analyst1");
            doc.MeasurementAggregateDocument.MeasurementDocument.Should().HaveCount(1);
        }
    }
}
