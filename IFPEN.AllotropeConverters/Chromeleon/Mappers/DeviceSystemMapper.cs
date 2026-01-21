using IFPEN.AllotropeConverters.AllotropeModels;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Thermo.Chromeleon.Sdk.Interfaces.Instruments.Symbols;

namespace Ifpen.AllotropeConverters.Chromeleon.Mappers
{
    /// <summary>
    /// Maps instrument symbol data to the Allotrope Device System Document.
    /// </summary>
    public class DeviceSystemMapper : IDeviceSystemMapper
    {
        private readonly IInstrumentDataProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceSystemMapper"/> class.
        /// </summary>
        /// <param name="provider">The instrument data provider.</param>
        public DeviceSystemMapper(IInstrumentDataProvider provider)
        {
            _provider = provider;
        }

        /// <inheritdoc />
        public DeviceSystemDocument Map(ISymbol rootSymbol)
        {
            string serial = _provider.GetSerialNumber(rootSymbol);
            string brand = _provider.GetManufacturer(rootSymbol);

            return new DeviceSystemDocument(
                assetManagementIdentifier: "N/A",
                deviceIdentifier: serial,
                modelNumber: null,
                equipmentSerialNumber: serial,
                firmwareVersion: null,
                description: null,
                brandName: brand,
                productManufacturer: brand,
                pumpModelNumber: null,
                detectorModelNumber: null,
                deviceDocument: null
            );
        }
    }
}
