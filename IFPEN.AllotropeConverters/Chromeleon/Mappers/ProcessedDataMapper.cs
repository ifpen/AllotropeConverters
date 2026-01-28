using System.Collections.Generic;
using System.Linq;
using IFPEN.AllotropeConverters.AllotropeModels;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Thermo.Chromeleon.Sdk.Interfaces.Data;
using Thermo.Chromeleon.Sdk.Interfaces.RawData;

namespace Ifpen.AllotropeConverters.Chromeleon.Mappers
{
    /// <summary>
    /// Maps processed peak results to the Allotrope Processed Data Document.
    /// </summary>
    public class ProcessedDataMapper : IProcessedDataMapper
    {
        private readonly IPeakDataProvider _peakProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessedDataMapper"/> class.
        /// </summary>
        /// <param name="peakProvider">The provider for peak data extraction.</param>
        public ProcessedDataMapper(IPeakDataProvider peakProvider)
        {
            _peakProvider = peakProvider;
        }

        /// <inheritdoc />
        public List<ProcessedDataDocument> Map(ISignal signal)
        {
            var injection = signal.Injection;
            if (injection == null) return new List<ProcessedDataDocument>();

            var peaks = _peakProvider.GetPeaks(injection, signal.Name);

            string signalUnit = signal.Metadata?.SignalAxis?.Unit ?? "arb";
            string areaUnit = $"{signalUnit} s";

            var list = new List<Peak>();
            foreach (var p in peaks)
            {
                list.Add(new Peak(
                    identifier: p.Index.ToString(),
                    writtenName: p.Name ?? $"Peak {p.Index}",
                    retentionTime: new PeakRetentionTime(p.RetentionTimeSeconds.Value, PeakRetentionTime.UnitEnum.S),
                    peakStart: new PeakPeakStart(p.StartTimeSeconds.Value, PeakPeakStart.UnitEnum.S),
                    peakEnd: new PeakPeakEnd(p.EndTimeSeconds.Value, PeakPeakEnd.UnitEnum.S),
                    peakArea: new PeakPeakArea(p.Area.Value, areaUnit),
                    peakHeight: new PeakPeakHeight(p.Height.Value, signalUnit),
                    peakWidthAtBaseline: new PeakPeakWidthAtBaseline(p.WidthBaselineSeconds.Value, PeakPeakWidthAtBaseline.UnitEnum.S),
                    peakWidthAtHalfHeight: null,
                    peakWidthAt5OfHeight: null,
                    asymmetryFactorMeasuredAt5Height: p.Asymmetry.HasValue ? new PeakAsymmetryFactorMeasuredAt5Height(p.Asymmetry.Value, PeakAsymmetryFactorMeasuredAt5Height.UnitEnum.Unitless) : null,
                    statisticalSkew: p.Skewness.HasValue ? new PeakStatisticalSkew(p.Skewness.Value, PeakStatisticalSkew.UnitEnum.Unitless) : null,
                    numberOfTheoreticalPlatesByTangentMethod: p.TheoreticalPlates.HasValue ? new PeakNumberOfTheoreticalPlatesByTangentMethod(p.TheoreticalPlates.Value, PeakNumberOfTheoreticalPlatesByTangentMethod.UnitEnum.Unitless) : null,
                    chromatographicPeakResolutionUsingBaselinePeakWidths: p.Resolution.HasValue ? new PeakChromatographicPeakResolutionUsingBaselinePeakWidths(p.Resolution.Value, PeakChromatographicPeakResolutionUsingBaselinePeakWidths.UnitEnum.Unitless) : null,
                    numberOfTheoreticalPlatesByPeakWidthAtHalfHeight: null,
                    capacityFactorChromatography: null,
                    relativePeakHeight: null,
                    relativePeakArea: null,
                    peakSelectivityChromatography: null,
                    chromatographicPeakResolutionUsingPeakWidthAtHalfHeight: null,
                    chromatographicPeakResolutionUsingStatisticalMoments: null
                ));
            }
            var peakList = new PeakList(list);

            return new List<ProcessedDataDocument>
            {
                new ProcessedDataDocument(peakList)
            };
        }
    }
}
