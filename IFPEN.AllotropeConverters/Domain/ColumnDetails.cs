namespace Ifpen.AllotropeConverters.Domain
{
    /// <summary>
    /// Represents a normalized data transfer object for chromatography column metadata.
    /// </summary>
    public class ColumnDetails
    {
        /// <summary>
        /// Gets or sets the description or chemistry type of the column.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the length of the column in meters.
        /// </summary>
        public double? LengthMeters { get; set; }

        /// <summary>
        /// Gets or sets the internal diameter of the column in millimeters.
        /// </summary>
        public double? InternalDiameterMm { get; set; }

        /// <summary>
        /// Gets or sets the film thickness or particle size in micrometers.
        /// </summary>
        public double? FilmThicknessMicrons { get; set; }
    }
}
