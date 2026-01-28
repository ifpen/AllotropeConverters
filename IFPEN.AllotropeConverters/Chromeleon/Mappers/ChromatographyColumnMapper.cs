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

            return new ChromatographyColumnDocument(
                chromatographyColumnSerialNumber: "N/A",
                chromatographyColumnChemistryType: details.Description,
                productManufacturer: _provider.GetManufacturer(rootSymbol),

                chromatographyColumnLength: details.LengthMeters.HasValue
                    ? new ChromatographyColumnDocumentChromatographyColumnLength(
                        value: details.LengthMeters.Value,
                        unit: ChromatographyColumnDocumentChromatographyColumnLength.UnitEnum.M) : null,

                columnInnerDiameter: details.InternalDiameterMm.HasValue
                    ? new ChromatographyColumnDocumentColumnInnerDiameter(
                        value: details.InternalDiameterMm.Value,
                        unit: ChromatographyColumnDocumentColumnInnerDiameter.UnitEnum.Mm) : null,

                chromatographyColumnParticleSize: details.FilmThicknessMicrons.HasValue
                    ? new ChromatographyColumnDocumentChromatographyColumnParticleSize(
                        value: details.FilmThicknessMicrons.Value,
                        unit: ChromatographyColumnDocumentChromatographyColumnParticleSize.UnitEnum.M) : null
            );
        }
    }
}
