using System.Globalization;
using System.Linq;
using System.Reflection;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Thermo.Chromeleon.Sdk.Interfaces.Instruments.Symbols;

namespace Ifpen.AllotropeConverters.Chromeleon.Infrastructure
{
    /// <summary>
    /// Implementation of <see cref="ISymbolReader"/> that handles reflection 
    /// and culture-invariant parsing for robust value extraction.
    /// </summary>
    public class RobustSymbolReader : ISymbolReader
    {
        /// <inheritdoc />
        public string ReadString(ISymbol root, string relativePath)
        {
            if (root == null) return null;
            var target = root.FindChild(relativePath);
            if (target == null) return null;
            return ExtractValueViaReflection(target);
        }

        /// <inheritdoc />
        public double? ReadDouble(ISymbol root, string relativePath)
        {
            var val = ReadString(root, relativePath);
            if (string.IsNullOrWhiteSpace(val)) return null;

            var normalized = val.Replace(',', '.');
            if (double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            return null;
        }

        private string ExtractValueViaReflection(ISymbol symbol)
        {
            try
            {
                var props = symbol.GetType().GetProperties();
                var prop = props.FirstOrDefault(p => p.Name == "OfflineValue" && p.CanRead)
                        ?? props.FirstOrDefault(p => p.Name == "OnlineValue" && p.CanRead)
                        ?? props.FirstOrDefault(p => p.Name == "Value" && p.CanRead);

                return prop?.GetValue(symbol)?.ToString();
            }
            catch { return null; }
        }
    }
}
