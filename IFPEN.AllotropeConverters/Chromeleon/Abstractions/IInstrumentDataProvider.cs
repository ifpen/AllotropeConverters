using Ifpen.AllotropeConverters.Domain;
using Thermo.Chromeleon.Sdk.Interfaces.Instruments.Symbols;

namespace Ifpen.AllotropeConverters.Chromeleon.Abstractions
{
    /// <summary>
    /// Defines a contract for retrieving instrument-specific metadata using vendor-specific logic.
    /// </summary>
    public interface IInstrumentDataProvider
    {
        /// <summary>
        /// Retrieves the equipment serial number from the symbol tree.
        /// </summary>
        /// <param name="rootSymbol">The root symbol of the instrument script.</param>
        /// <returns>The serial number, or "N/A" if not found.</returns>
        string GetSerialNumber(ISymbol rootSymbol);

        /// <summary>
        /// Retrieves the manufacturer name based on the symbol tree structure.
        /// </summary>
        /// <param name="rootSymbol">The root symbol of the instrument script.</param>
        /// <returns>The manufacturer name, or "Unknown".</returns>
        string GetManufacturer(ISymbol rootSymbol);

        /// <summary>
        /// Retrieves the column details (length, diameter, etc.) from the symbol tree.
        /// </summary>
        /// <param name="rootSymbol">The root symbol of the instrument script.</param>
        /// <returns>A <see cref="ColumnDetails"/> object, or null if no column is found.</returns>
        ColumnDetails GetColumnDetails(ISymbol rootSymbol);
    }
}
