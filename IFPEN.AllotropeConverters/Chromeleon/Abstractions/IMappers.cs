using System.Collections.Generic;
using IFPEN.AllotropeConverters.AllotropeModels;
using Thermo.Chromeleon.Sdk.Interfaces.Data;
using Thermo.Chromeleon.Sdk.Interfaces.Instruments.Symbols;
using Thermo.Chromeleon.Sdk.Interfaces.RawData;

namespace Ifpen.AllotropeConverters.Chromeleon.Abstractions
{
    /// <summary>
    /// Maps the device system information to the Allotrope model.
    /// </summary>
    public interface IDeviceSystemMapper
    {
        /// <summary>Maps the device system document.</summary>
        /// <param name="rootSymbol">The root symbol.</param>
        /// <returns>The mapped document.</returns>
        DeviceSystemDocument Map(ISymbol rootSymbol);
    }

    /// <summary>
    /// Maps the chromatography column information to the Allotrope model.
    /// </summary>
    public interface IChromatographyColumnMapper
    {
        /// <summary>Maps the column document.</summary>
        /// <param name="rootSymbol">The root symbol.</param>
        /// <returns>The mapped document.</returns>
        ChromatographyColumnDocument Map(ISymbol rootSymbol);
    }

    /// <summary>
    /// Maps the raw signal data (chromatogram) to the Allotrope Data Cube.
    /// </summary>
    public interface IDataCubeMapper
    {
        /// <summary>Maps the data cube.</summary>
        /// <param name="signal">The raw signal.</param>
        /// <returns>The mapped data cube.</returns>
        ChromatogramDataCube Map(ISignal signal);
    }

    /// <summary>
    /// Maps the sample information to the Allotrope model.
    /// </summary>
    public interface ISampleMapper
    {
        /// <summary>Maps the sample document.</summary>
        /// <param name="injection">The injection data.</param>
        /// <returns>The mapped document.</returns>
        SampleDocument Map(IInjection injection);
    }

    /// <summary>
    /// Maps the processed results (peaks) to the Allotrope model.
    /// </summary>
    public interface IProcessedDataMapper
    {
        /// <summary>Maps the processed data document.</summary>
        /// <param name="signal">The signal containing results.</param>
        /// <returns>A list of processed data documents.</returns>
        List<ProcessedDataDocument> Map(ISignal signal);
    }
}
