namespace Ifpen.AllotropeConverters.Domain
{
    /// <summary>
    /// Represents a normalized data transfer object for a chromatographic peak.
    /// Contains physical values without SDK dependencies.
    /// </summary>
    public class PeakData
    {
        /// <summary>
        /// Gets or sets the 1-based index of the peak in the signal.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the name of the peak.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the retention time in seconds.
        /// </summary>
        public double? RetentionTimeSeconds { get; set; }

        /// <summary>
        /// Gets or sets the start time of the peak in seconds.
        /// </summary>
        public double? StartTimeSeconds { get; set; }

        /// <summary>
        /// Gets or sets the end time of the peak in seconds.
        /// </summary>
        public double? EndTimeSeconds { get; set; }

        /// <summary>
        /// Gets or sets the calculated area of the peak.
        /// Unit depends on the signal unit multiplied by time.
        /// </summary>
        public double? Area { get; set; }

        /// <summary>
        /// Gets or sets the height of the peak.
        /// Unit depends on the signal unit.
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Gets or sets the width of the peak at the baseline in seconds.
        /// </summary>
        public double? WidthBaselineSeconds { get; set; }

        /// <summary>
        /// Gets or sets the chromatographic resolution (Pre-2022 USP).
        /// </summary>
        public double? ResolutionBaseline { get; set; }

        /// <summary>
        /// Gets or sets the chromatographic resolution (EP/USP).
        /// </summary>
        public double? ResolutionHalfHeight { get; set; }

        /// <summary>
        /// Gets or sets the chromatographic resolution (Statistical Moments).
        /// </summary>
        public double? ResolutionStatisticalMoments { get; set; }

        /// <summary>
        /// Gets or sets the number of theoretical plates (Tangent method).
        /// </summary>
        public double? TheoreticalPlatesTangent { get; set; }

        /// <summary>
        /// Gets or sets the number of theoretical plates (Half Height method).
        /// </summary>
        public double? TheoreticalPlatesHalfHeight { get; set; }

        /// <summary>
        /// Gets or sets the asymmetry factor.
        /// </summary>
        public double? Asymmetry { get; set; }

        /// <summary>
        /// Gets or sets the capacity factor.
        /// </summary>
        public double? CapacityFactor { get; set; }

        /// <summary>
        /// Gets or sets the relative height (relative to total).
        /// </summary>
        public double? RelativeHeight { get; set; }

        /// <summary>
        /// Gets or sets the relative area (relative to total).
        /// </summary>
        public double? RelativeArea { get; set; }

        /// <summary>
        /// Gets or sets the statistical skewness of the peak.
        /// </summary>
        public double? Skewness { get; set; }

        /// <summary>
        /// Gets or sets the width of the peak at half height in seconds.
        /// </summary>
        public double? WidthHalfHeightSeconds { get; set; }

        /// <summary>
        /// Gets or sets the width of the peak at 5% height in seconds.
        /// </summary>
        public double? Width5PercentHeightSeconds { get; set; }
    }
}
