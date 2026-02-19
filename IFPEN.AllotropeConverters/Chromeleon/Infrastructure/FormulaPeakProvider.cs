using System.Collections.Generic;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Ifpen.AllotropeConverters.Domain;
using Thermo.Chromeleon.Sdk.Common;
using Thermo.Chromeleon.Sdk.Interfaces.Data;
using Thermo.Chromeleon.Sdk.Interfaces.Evaluation;

namespace Ifpen.AllotropeConverters.Chromeleon.Infrastructure
{
    /// <summary>
    /// Implementation of <see cref="IPeakDataProvider"/> using the Chromeleon Reporting Engine (Formulas)
    /// to extract peak results.
    /// </summary>
    public class FormulaPeakProvider : IPeakDataProvider
    {
        /// <inheritdoc />
        public List<PeakData> GetPeaks(IInjection injection, string signalName)
        {
            var peaks = new List<PeakData>();
            if (injection == null || string.IsNullOrEmpty(signalName)) return peaks;

            try
            {
                var itemFactory = CmSdk.GetItemFactory();
                int peakIndex = 0;

                while (true)
                {
                    var context = itemFactory.CreateEvaluationContext(signalName, peakIndex);

                    var check = injection.Evaluate("peak.number", context);
                    if (check.IsError) break;

                    peaks.Add(new PeakData
                    {
                        Index = peakIndex,
                        Name = EvaluateString(injection, "peak.name", context),
                        RetentionTimeSeconds = EvaluateDouble(injection, "peak.retention_time", context) * 60.0,
                        StartTimeSeconds = EvaluateDouble(injection, "peak.start_time", context) * 60.0,
                        EndTimeSeconds = EvaluateDouble(injection, "peak.stop_time", context) * 60.0,
                        Area = EvaluateDouble(injection, "peak.area", context),
                        Height = EvaluateDouble(injection, "peak.height", context),
                        WidthBaselineSeconds = EvaluateDouble(injection, "peak.width", context) * 60.0,
                        ResolutionBaseline = EvaluateDouble(injection, "peak.resolution(\"pre2022-usp\")", context),
                        ResolutionHalfHeight = EvaluateDouble(injection, "peak.resolution(\"ep/usp\")", context),
                        ResolutionStatisticalMoments = EvaluateDouble(injection, "peak.resolution(\"sm\")", context),
                        TheoreticalPlatesTangent = EvaluateDouble(injection, "peak.theoretical_plates(\"pre2022-usp\")", context),
                        TheoreticalPlatesHalfHeight = EvaluateDouble(injection, "peak.theoretical_plates(\"ep/usp\")", context),
                        Asymmetry = EvaluateDouble(injection, "peak.asymmetry", context),
                        CapacityFactor = EvaluateDouble(injection, "peak.kValue", context),
                        RelativeHeight = EvaluateDouble(injection, "peak.rel_height(\"total\")", context),
                        RelativeArea = EvaluateDouble(injection, "peak.rel_area(\"total\")", context),
                        Skewness = EvaluateDouble(injection, "peak.skewness", context),
                        WidthHalfHeightSeconds = EvaluateDouble(injection, "peak.width(50)", context) * 60.0,
                        Width5PercentHeightSeconds = EvaluateDouble(injection, "peak.width(5)", context) * 60.0
                    });

                    peakIndex++;
                }
            }
            catch 
            { 
                // Ignore errors during peak traversal
            }

            return peaks;
        }

        private double? EvaluateDouble(IInjection injection, string formula, IEvaluationContext context)
        {
            var res = injection.Evaluate(formula, context);
            return (res != null && !res.IsError && res.IsNumeric) ? (double?)res.NumericValue : null;
        }

        private string EvaluateString(IInjection injection, string formula, IEvaluationContext context)
        {
            var res = injection.Evaluate(formula, context);
            return (res != null && !res.IsError && res.IsString) ? res.StringValue : null;
        }
    }
}
