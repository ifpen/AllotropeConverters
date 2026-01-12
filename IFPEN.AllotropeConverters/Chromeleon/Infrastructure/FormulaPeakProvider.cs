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
                int peakIndex = 1;

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
                        Resolution = EvaluateDouble(injection, "peak.resolution", context),
                        TheoreticalPlates = EvaluateDouble(injection, "peak.theoretical_plates", context),
                        Asymmetry = EvaluateDouble(injection, "peak.asymmetry", context),
                        Skewness = EvaluateDouble(injection, "peak.skewness", context)
                    });

                    peakIndex++;
                }
            }
            catch { /* Silent */ }

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
