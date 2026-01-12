using IFPEN.AllotropeConverters.AllotropeModels;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Thermo.Chromeleon.Sdk.Interfaces.Instruments.Symbols;

namespace Ifpen.AllotropeConverters.Chromeleon.Mappers
{
    /// <summary>
    /// Maps instrument symbol data to the Allotrope Chromatography Column Document.
    /// </summary>
    public class ChromatographyColumnMapper : IChromatographyColumnMapper
    {
        private readonly IInstrumentDataProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromatographyColumnMapper"/> class.
        /// </summary>
        /// <param name="provider">The instrument data provider.</param>
        public ChromatographyColumnMapper(IInstrumentDataProvider provider)
        {
            _provider = provider;
        }

        /// <inheritdoc />
        public ChromatographyColumnDocument Map(ISymbol rootSymbol)
        {
            var details = _provider.GetColumnDetails(rootSymbol);

            if (details == null)
                return new ChromatographyColumnDocument { ChromatographyColumnSerialNumber = "N/A" };

            return new ChromatographyColumnDocument
            {
                ChromatographyColumnSerialNumber = "N/A",
                ChromatographyColumnChemistryType = details.Description,
                ProductManufacturer = _provider.GetManufacturer(rootSymbol),

                ChromatographyColumnLength = details.LengthMeters.HasValue
                    ? new ChromatographyColumnDocumentChromatographyColumnLength { 
                        Value = details.LengthMeters.Value,
                        Unit = ChromatographyColumnDocumentChromatographyColumnLength.UnitEnum.M } : null,

                ColumnInnerDiameter = details.InternalDiameterMm.HasValue
                    ? new ChromatographyColumnDocumentColumnInnerDiameter {
                        Value = details.InternalDiameterMm.Value,
                        Unit = ChromatographyColumnDocumentColumnInnerDiameter.UnitEnum.Mm } : null,

                ChromatographyColumnParticleSize = details.FilmThicknessMicrons.HasValue
                    ? new ChromatographyColumnDocumentChromatographyColumnParticleSize { 
                        Value = details.FilmThicknessMicrons.Value,
                        Unit = ChromatographyColumnDocumentChromatographyColumnParticleSize.UnitEnum.M } : null
            };
        }
    }
}
