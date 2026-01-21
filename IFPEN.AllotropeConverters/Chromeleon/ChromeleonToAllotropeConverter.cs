using System;
using System.Collections.Generic;
using System.Linq;
using IFPEN.AllotropeConverters.AllotropeModels;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Ifpen.AllotropeConverters.Chromeleon.Infrastructure;
using Ifpen.AllotropeConverters.Chromeleon.Mappers;
using Thermo.Chromeleon.Sdk.Common;
using Thermo.Chromeleon.Sdk.Interfaces.Data;
using Thermo.Chromeleon.Sdk.Interfaces.Instruments.Symbols;
using Thermo.Chromeleon.Sdk.Interfaces.RawData;

namespace Ifpen.AllotropeConverters.Chromeleon
{
    /// <summary>
    /// Provides functionality to convert Chromeleon injections to Allotrope Gas Chromatography models.
    /// Manages the SDK lifecycle and data mapping orchestration.
    /// </summary>
    public class ChromeleonToAllotropeConverter : IDisposable
    {
        private const string ManifestUrl = "http://purl.allotrope.org/manifests/gas-chromatography/REC/2025/06/gas-chromatography.tabular.manifest";

        private readonly IDeviceSystemMapper _deviceMapper;
        private readonly IChromatographyColumnMapper _columnMapper;
        private readonly ISampleMapper _sampleMapper;
        private readonly IDataCubeMapper _dataCubeMapper;
        private readonly IProcessedDataMapper _processedDataMapper;

        private readonly IDisposable _managedScope;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeleonToAllotropeConverter"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor initializes a new <see cref="CmSdkScope"/> and performs a logon.
        /// It should only be used when running as a standalone application.
        /// </remarks>
        public ChromeleonToAllotropeConverter()
        {
            _managedScope = new CmSdkScope();
            CmSdk.Logon.DoLogon();

            var reader = new RobustSymbolReader();
            var instrumentProvider = new MultiVendorInstrumentProvider(reader);
            var peakProvider = new FormulaPeakProvider();

            _deviceMapper = new DeviceSystemMapper(instrumentProvider);
            _columnMapper = new ChromatographyColumnMapper(instrumentProvider);
            _sampleMapper = new SampleMapper();
            _dataCubeMapper = new DataCubeMapper();
            _processedDataMapper = new ProcessedDataMapper(peakProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeleonToAllotropeConverter"/> class with an existing scope.
        /// </summary>
        /// <param name="activeScope">The active SDK scope. This scope will not be disposed by the converter.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="activeScope"/> is null.</exception>
        public ChromeleonToAllotropeConverter(CmSdkScope activeScope)
        {
            if (activeScope == null) throw new ArgumentNullException(nameof(activeScope));

            _managedScope = null;

            var reader = new RobustSymbolReader();
            var instrumentProvider = new MultiVendorInstrumentProvider(reader);
            var peakProvider = new FormulaPeakProvider();

            _deviceMapper = new DeviceSystemMapper(instrumentProvider);
            _columnMapper = new ChromatographyColumnMapper(instrumentProvider);
            _sampleMapper = new SampleMapper();
            _dataCubeMapper = new DataCubeMapper();
            _processedDataMapper = new ProcessedDataMapper(peakProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeleonToAllotropeConverter"/> class with injected dependencies.
        /// </summary>
        /// <param name="deviceMapper">The device system mapper.</param>
        /// <param name="columnMapper">The chromatography column mapper.</param>
        /// <param name="sampleMapper">The sample mapper.</param>
        /// <param name="dataCubeMapper">The data cube mapper.</param>
        /// <param name="processedDataMapper">The processed data mapper.</param>
        public ChromeleonToAllotropeConverter(
            IDeviceSystemMapper deviceMapper,
            IChromatographyColumnMapper columnMapper,
            ISampleMapper sampleMapper,
            IDataCubeMapper dataCubeMapper,
            IProcessedDataMapper processedDataMapper)
        {
            _deviceMapper = deviceMapper;
            _columnMapper = columnMapper;
            _sampleMapper = sampleMapper;
            _dataCubeMapper = dataCubeMapper;
            _processedDataMapper = processedDataMapper;
            _managedScope = null;
        }

        /// <summary>
        /// Converts the specified Chromeleon injection into an Allotrope Gas Chromatography model.
        /// </summary>
        /// <param name="injection">The Chromeleon injection to convert.</param>
        /// <returns>The generated Allotrope model.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="injection"/> is null.</exception>
        public GasChromatographySimpleModel Convert(IInjection injection)
        {
            if (injection == null) throw new ArgumentNullException(nameof(injection));

            ISymbol rootSymbol = injection.InstrumentMethod?.Script?.RootSymbol;

            return new GasChromatographySimpleModel
            {
                AsmManifest = GasChromatographySimpleModel.AsmManifestEnum.HttpPurlAllotropeOrgManifestsGasChromatographyREC202506GasChromatographyTabularManifest,
                GasChromatographyAggregateDocument = new GasChromatographyAggregateDocument(
                    deviceSystemDocument: _deviceMapper.Map(rootSymbol),
                    gasChromatographyDocument: new List<GasChromatographyDocument>
                    {
                        new GasChromatographyDocument(
                            analyst: GetAnalyst(injection),
                            submitter: null,
                            deviceMethodIdentifier: injection.InstrumentMethodName.Value,
                            measurementAggregateDocument: MapMeasurements(injection, rootSymbol),
                            diagnosticTraceAggregateDocument: null)
                    }
                )
            };
        }

        private MeasurementAggregateDocument MapMeasurements(IInjection injection, ISymbol rootSymbol)
        {
            var list = new List<MeasurementDocument>();
            if (injection.Signals != null)
            {
                foreach (ISignal signal in injection.Signals)
                {
                    list.Add(new MeasurementDocument(
                        measurementIdentifier: signal.Id.ToString(),
                        detectionType: signal.Metadata?.DetectorDevice ?? "Unknown",
                        chromatogramDataCube: _dataCubeMapper.Map(signal),
                        chromatographyColumnDocument: _columnMapper.Map(rootSymbol),
                        sampleDocument: _sampleMapper.Map(injection),
                        processedDataAggregateDocument: new ProcessedDataAggregateDocument(
                            processedDataDocument: _processedDataMapper.Map(signal)
                        ),
                        deviceControlAggregateDocument: new DeviceControlAggregateDocument(
                            deviceControlDocument: new List<DeviceControlDocument>
                            {
                                new DeviceControlDocument(deviceType: signal.Metadata?.DetectorDevice ?? "Unknown")
                            }
                        ),
                        injectionDocument: new InjectionDocument(
                            injectionIdentifier: injection.Name,
                            injectionTime: injection.InjectTime.Value.UtcDateTime,
                            injectionVolumeSetting: new InjectionDocumentInjectionVolumeSetting(
                                value: injection.InjectionVolume.Value.Value,
                                unit: InjectionDocumentInjectionVolumeSetting.UnitEnum.Micro_L
                            )
                        )
                    ));
                }
            }
            return new MeasurementAggregateDocument(measurementDocument: list);
        }

        private string GetAnalyst(IInjection injection) => injection.Signals?.Cast<ISignal>().FirstOrDefault()?.Metadata?.User ?? "N/A";

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ChromeleonToAllotropeConverter"/> 
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _managedScope?.Dispose();
            }

            _disposed = true;
        }
    }
}
