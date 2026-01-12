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
            if (peaks == null || !peaks.Any()) return new List<ProcessedDataDocument>();

            string signalUnit = signal.Metadata?.SignalAxis?.Unit ?? "arb";
            string areaUnit = $"{signalUnit} s";

            var peakList = new PeakList
            {
                Peak = peaks.Select(p => new Peak
                {
                    Identifier = p.Index.ToString(),
                    WrittenName = p.Name ?? $"Peak {p.Index}",

                    RetentionTime = new PeakRetentionTime
                    {
                        Value = p.RetentionTimeSeconds.Value,
                        Unit = PeakRetentionTime.UnitEnum.S
                    },

                    PeakStart = new PeakPeakStart
                    {
                        Value = p.StartTimeSeconds.Value,
                        Unit = PeakPeakStart.UnitEnum.S
                    },

                    PeakEnd = new PeakPeakEnd
                    {
                        Value = p.EndTimeSeconds.Value,
                        Unit = PeakPeakEnd.UnitEnum.S
                    },

                    PeakArea = new PeakPeakArea
                    {
                        Value = p.Area.Value,
                        Unit = areaUnit
                    },


                    PeakHeight = new PeakPeakHeight
                    {
                        Value = p.Height.Value,
                        Unit = signalUnit
                    },

                    PeakWidthAtBaseline = new PeakPeakWidthAtBaseline
                    {
                        Value = p.WidthBaselineSeconds.Value,
                        Unit = PeakPeakWidthAtBaseline.UnitEnum.S
                    },

                    PeakWidthAtHalfHeight = null,
                    PeakWidthAt5OfHeight = null,

                    AsymmetryFactorMeasuredAt5Height = new PeakAsymmetryFactorMeasuredAt5Height
                    {
                        Value = p.Asymmetry.Value,
                        Unit = PeakAsymmetryFactorMeasuredAt5Height.UnitEnum.Unitless
                    },
                    StatisticalSkew = new PeakStatisticalSkew
                    {
                        Value = p.Skewness.Value,
                        Unit = PeakStatisticalSkew.UnitEnum.Unitless
                    },

                    NumberOfTheoreticalPlatesByTangentMethod = new PeakNumberOfTheoreticalPlatesByTangentMethod
                    {
                        Value = p.TheoreticalPlates.Value,
                        Unit = PeakNumberOfTheoreticalPlatesByTangentMethod.UnitEnum.Unitless
                    },

                    ChromatographicPeakResolutionUsingBaselinePeakWidths = new PeakChromatographicPeakResolutionUsingBaselinePeakWidths
                    {
                        Value = p.Resolution.Value,
                        Unit = PeakChromatographicPeakResolutionUsingBaselinePeakWidths.UnitEnum.Unitless
                    }}).ToList()
            };

            return new List<ProcessedDataDocument>
            {
                new ProcessedDataDocument { PeakList = peakList }
            };
        }
    }
}
