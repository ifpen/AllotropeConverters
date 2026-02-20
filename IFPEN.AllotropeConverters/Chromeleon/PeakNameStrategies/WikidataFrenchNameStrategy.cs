using System;
using System.Linq;
using System.Net.Http;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Ifpen.AllotropeConverters.Chromeleon.PeakNameStrategies
{
    /// <summary>
    /// Strategy that retrieves the French name of a compound from Wikidata using SPARQL.
    /// Searches for any label or alias matching the input name (case-insensitive) and returns the French label.
    /// </summary>
    public class WikidataFrenchNameStrategy : IPeakNameMappingStrategy
    {
        private const string SparqlEndpoint = "https://query.wikidata.org/sparql";
        private readonly HttpClient _httpClient;
        private readonly ILogger<WikidataFrenchNameStrategy> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikidataFrenchNameStrategy"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpClient">The HTTP client (optional).</param>
        public WikidataFrenchNameStrategy(ILogger<WikidataFrenchNameStrategy> logger, HttpClient httpClient = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
        }

        /// <inheritdoc />
        public string MapName(string originalName)
        {
            if (string.IsNullOrEmpty(originalName)) return originalName;

            try
            {
                string frenchName = GetFrenchNameFromWikidata(originalName);

                if (!string.IsNullOrEmpty(frenchName))
                {
                    _logger.LogInformation("Found French name for {Name}: {FrenchName}", originalName, frenchName);
                    return frenchName;
                }
                else
                {
                    _logger.LogWarning("No French name found for {Name}", originalName);
                    return originalName;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying Wikidata for {Name}: {Message}", originalName, ex.Message);
                return originalName;
            }
        }

        private string GetFrenchNameFromWikidata(string originalName)
        {
            // Escape special characters to prevent SPARQL injection
            string safeName = originalName.Replace("\"", "\\\"");
 
            // Use wikibase:mwapi EntitySearch for efficient, case-insensitive item lookup.
            // This approach is significantly faster than RDFS label scanning and 
            // provides robust matching for chemical compound names and aliases.
            string query = $@"
SELECT ?frLabel WHERE {{
  SERVICE wikibase:mwapi {{
      bd:serviceParam wikibase:endpoint ""www.wikidata.org"";
                      wikibase:api ""EntitySearch"";
                      mwapi:search ""{safeName}"";
                      mwapi:language ""en"".
      ?item wikibase:apiOutputItem mwapi:item.
  }}
  ?item rdfs:label ?frLabel .
  FILTER (LANG(?frLabel) = ""fr"")
}} LIMIT 1";
 
 
            var request = new HttpRequestMessage(HttpMethod.Get, $"{SparqlEndpoint}?query={Uri.EscapeDataString(query)}&format=json");
 
            request.Headers.Add("User-Agent", "IFPEN-AllotropeConverters/1.0 (internal tool)");


            var response = _httpClient.SendAsync(request).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Wikidata returned status code {StatusCode}", response.StatusCode);
                return null;
            }

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var json = JObject.Parse(content);
            var bindings = json["results"]?["bindings"] as JArray;

            if (bindings != null && bindings.Count > 0)
            {
                return bindings[0]["frLabel"]?["value"]?.ToString();
            }

            return null;
        }
    }
}
