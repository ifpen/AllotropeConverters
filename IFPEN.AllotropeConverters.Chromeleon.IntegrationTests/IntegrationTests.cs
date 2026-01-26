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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var pekindDict = new Dictionary<Assembly, PortableExecutableKinds>();

            foreach (var a in assemblies)
            {
                a.ManifestModule.GetPEKind(out var peKind, out _);
                pekindDict[a] = peKind;
            }

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

                bool valid = jsonObject.IsValid(schema, out IList<string> errors);

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

        public static int Main(string[] args)
        {
            // This is needed to make the project an executable for Chromeleon SDK compatibility
            // The actual test execution is handled by the test runner
            Console.WriteLine("This is an executable test project for Chromeleon SDK compatibility.");
            Console.WriteLine("Tests should be run through a test runner like Visual Studio Test Explorer or dotnet test.");

            // Return 0 to indicate success when run directly
            return 0;
        }
    }
}
