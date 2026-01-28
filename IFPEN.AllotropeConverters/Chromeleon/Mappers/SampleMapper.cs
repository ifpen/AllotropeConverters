using IFPEN.AllotropeConverters.AllotropeModels;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Thermo.Chromeleon.Sdk.Interfaces.Data;

namespace Ifpen.AllotropeConverters.Chromeleon.Mappers
{
    /// <summary>
    /// Maps injection metadata to the Allotrope Sample Document.
    /// </summary>
    public class SampleMapper : ISampleMapper
    {
        /// <inheritdoc />
        public SampleDocument Map(IInjection injection)
        {
            return new SampleDocument(
                sampleIdentifier: injection.Name,
                description: injection.Comment.Value,
                writtenName: injection.Name,
                batchIdentifier: "N/A"
            );
        }
    }
}
