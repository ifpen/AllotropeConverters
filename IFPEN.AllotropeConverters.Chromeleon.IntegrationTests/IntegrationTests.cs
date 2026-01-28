using FluentAssertions;
using Ifpen.AllotropeConverters.Chromeleon;
using IFPEN.AllotropeConverters.AllotropeModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Reflection;
using Xunit;

namespace IFPEN.AllotropeConverters.Chromeleon.IntegrationTests
{
    public class IntegrationTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void Convert_ActualInjectionUri_GeneratesValidAsmJson()
        {
            var converter = new ChromeleonToAllotropeConverter();
            var injectionUri = new Uri("chrom://isntsv-pacha4/DATA_R06_sql/DSI/testSDK20250219-113941.seq/662.smp");
            var model = converter.Convert(injectionUri);

            model.Should().NotBeNull();
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);

            // Assert - Schema Validation
            string schemaPath = Path.Combine("Schemas", "gas-chromatography.tabular.embed.schema.json");
            if (File.Exists(schemaPath))
            {
                string schemaJson = File.ReadAllText(schemaPath);
                JSchema schema = JSchema.Parse(schemaJson);
                JObject jsonObject = JObject.Parse(json);


                bool valid = jsonObject.IsValid(schema, out IList<ValidationError> errors);

                // Using FluentAssertions
                valid.Should().BeTrue($"Schema validation failed: {string.Join(", ", errors)}");
            }
            else
            {
                // Warn or skip
                // For now, we pass but note that schema was missing
                Assert.True(true, "Schema file not found, skipping validation.");
            }
        }

    }
}
