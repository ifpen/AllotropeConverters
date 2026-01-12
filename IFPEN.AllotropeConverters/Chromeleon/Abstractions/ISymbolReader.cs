using Thermo.Chromeleon.Sdk.Interfaces.Instruments.Symbols;

namespace Ifpen.AllotropeConverters.Chromeleon.Abstractions
{
    /// <summary>
    /// Defines a contract for reading values safely from the Chromeleon symbol tree.
    /// </summary>
    public interface ISymbolReader
    {
        /// <summary>
        /// Reads a string value from a symbol at the specified relative path.
        /// </summary>
        /// <param name="root">The root symbol to start the search from.</param>
        /// <param name="relativePath">The dot-separated path to the child symbol.</param>
        /// <returns>The string value, or null if not found.</returns>
        string ReadString(ISymbol root, string relativePath);

        /// <summary>
        /// Reads a double value from a symbol at the specified relative path.
        /// Handles culture-specific formatting (e.g., decimal commas).
        /// </summary>
        /// <param name="root">The root symbol to start the search from.</param>
        /// <param name="relativePath">The dot-separated path to the child symbol.</param>
        /// <returns>The double value, or null if not found or invalid.</returns>
        double? ReadDouble(ISymbol root, string relativePath);
    }
}
