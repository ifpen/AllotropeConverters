using System.Collections.Generic;
using Ifpen.AllotropeConverters.Domain;
using Thermo.Chromeleon.Sdk.Interfaces.Data;

namespace Ifpen.AllotropeConverters.Chromeleon.Abstractions
{
    /// <summary>
    /// Defines a contract for extracting processed peak data from an injection.
    /// </summary>
    public interface IPeakDataProvider
    {
        /// <summary>
        /// Retrieves a list of peaks for a specific signal in an injection.
        /// </summary>
        /// <param name="injection">The injection containing the results.</param>
        /// <param name="signalName">The name of the signal channel.</param>
        /// <returns>A list of <see cref="PeakData"/> objects.</returns>
        List<PeakData> GetPeaks(IInjection injection, string signalName);
    }
}
