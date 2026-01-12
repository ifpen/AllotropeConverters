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
        /// Gets or sets the chromatographic resolution (USP/EP).
        /// </summary>
        public double? Resolution { get; set; }

        /// <summary>
        /// Gets or sets the number of theoretical plates.
        /// </summary>
        public double? TheoreticalPlates { get; set; }

        /// <summary>
        /// Gets or sets the asymmetry factor.
        /// </summary>
        public double? Asymmetry { get; set; }

        /// <summary>
        /// Gets or sets the statistical skewness of the peak.
        /// </summary>
        public double? Skewness { get; set; }
    }
}
