using System.Collections.Generic;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Ifpen.AllotropeConverters.Domain;
using Thermo.Chromeleon.Sdk.Interfaces.Instruments.Symbols;

namespace Ifpen.AllotropeConverters.Chromeleon.Infrastructure
{
    /// <summary>
    /// Implementation of <see cref="IInstrumentDataProvider"/> supporting multiple vendors (Agilent, Thermo)
    /// by searching through known symbol paths.
    /// </summary>
    public class MultiVendorInstrumentProvider : IInstrumentDataProvider
    {
        private readonly ISymbolReader _reader;
        private readonly string[] _serialPaths = { "Agilent.GC.SerialNo", "GC.SerialNo", "System.SerialNo", "HP.GC.SerialNo" };
        private readonly string[] _columnPaths = { "Agilent.GC.FrontColumn", "Agilent.GC.BackColumn", "GC.FrontColumn", "GC.Column" };

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiVendorInstrumentProvider"/> class.
        /// </summary>
        /// <param name="reader">The symbol reader to use for value extraction.</param>
        public MultiVendorInstrumentProvider(ISymbolReader reader)
        {
            _reader = reader;
        }

        /// <inheritdoc />
        public string GetSerialNumber(ISymbol rootSymbol)
        {
            return FindFirstValidString(rootSymbol, _serialPaths) ?? "N/A";
        }

        /// <inheritdoc />
        public string GetManufacturer(ISymbol rootSymbol)
        {
            if (rootSymbol == null) return "Unknown";
            if (rootSymbol.FindChild("Agilent") != null) return "Agilent";
            if (rootSymbol.FindChild("Thermo") != null) return "Thermo Fisher Scientific";
            return "Unknown";
        }

        /// <inheritdoc />
        public ColumnDetails GetColumnDetails(ISymbol rootSymbol)
        {
            string basePath = FindBasePath(rootSymbol, _columnPaths);
            if (basePath == null) return null;

            return new ColumnDetails
            {
                Description = _reader.ReadString(rootSymbol, $"{basePath}.Description"),
                LengthMeters = _reader.ReadDouble(rootSymbol, $"{basePath}.Length"),
                InternalDiameterMm = _reader.ReadDouble(rootSymbol, $"{basePath}.NominalID"),
                FilmThicknessMicrons = _reader.ReadDouble(rootSymbol, $"{basePath}.FilmThickness")
            };
        }

        private string FindFirstValidString(ISymbol root, IEnumerable<string> paths)
        {
            if (root == null) return null;
            foreach (var path in paths)
            {
                var val = _reader.ReadString(root, path);
                if (!string.IsNullOrWhiteSpace(val)) return val;
            }
            return null;
        }

        private string FindBasePath(ISymbol root, IEnumerable<string> paths)
        {
            if (root == null) return null;
            foreach (var path in paths)
            {
                if (_reader.ReadString(root, $"{path}.Description") != null ||
                    _reader.ReadString(root, $"{path}.Length") != null)
                {
                    return path;
                }
            }
            return null;
        }
    }
}
