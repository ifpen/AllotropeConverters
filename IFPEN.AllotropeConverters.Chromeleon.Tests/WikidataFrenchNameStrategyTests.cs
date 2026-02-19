using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Ifpen.AllotropeConverters.Chromeleon.PeakNameStrategies;
using Moq;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests
{
    public class WikidataFrenchNameStrategyTests
    {
        [Fact]
        public void WikidataStrategy_ThrowsOnNullLogger()
        {
            Action act = () => new WikidataFrenchNameStrategy(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WikidataStrategy_ReturnsFrenchName_WhenFound()
        {
            // Arrange: Mock SPARQL response
            string safeName = "Enanthylic acid";
            string sparqlQuery = $@"
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

            string url = $"https://query.wikidata.org/sparql?query={Uri.EscapeDataString(sparqlQuery)}&format=json";

            var handler = new MockHttpMessageHandler(new Dictionary<string, HttpResponseMessage>
            {
                {
                    $"GET:{url}",
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{
                            ""head"": { ""vars"": [ ""frLabel"" ] },
                            ""results"": {
                                ""bindings"": [
                                    {
                                        ""frLabel"": { ""xml:lang"": ""fr"", ""type"": ""literal"", ""value"": ""acide heptanoïque"" }
                                    }
                                ]
                            }
                        }")
                    }
                }
            });

            var loggerMock = new Mock<ILogger<WikidataFrenchNameStrategy>>();
            var httpClient = new HttpClient(handler);
            var strategy = new WikidataFrenchNameStrategy(loggerMock.Object, httpClient);

            // Act
            var result = strategy.MapName("Enanthylic acid");

            // Assert
            result.Should().Be("acide heptanoïque");
            
            // Verify logging
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Found French name")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void WikidataStrategy_ReturnsOriginal_WhenNoMatch()
        {
            var safeName = "UnknownCompound";
            var sparqlQuery = $@"
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

            var url = $"https://query.wikidata.org/sparql?query={Uri.EscapeDataString(sparqlQuery)}&format=json";

            var handler = new MockHttpMessageHandler(new Dictionary<string, HttpResponseMessage>
            {
                {
                    $"GET:{url}",
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{ ""head"": { ""vars"": [ ""frLabel"" ] }, ""results"": { ""bindings"": [] } }") 
                    }
                }
            });

            var loggerMock = new Mock<ILogger<WikidataFrenchNameStrategy>>();
            var httpClient = new HttpClient(handler);
            var strategy = new WikidataFrenchNameStrategy(loggerMock.Object, httpClient);

            var result = strategy.MapName("UnknownCompound");
            result.Should().Be("UnknownCompound");

            loggerMock.Verify(
               x => x.Log(
                   LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No French name found")),
                   null,
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once);
        }

        [Fact]
        public void WikidataStrategy_ReturnsOriginal_WhenApiError()
        {
            var safeName = "ErrorCompound";
            var sparqlQuery = $@"
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

            var url = $"https://query.wikidata.org/sparql?query={Uri.EscapeDataString(sparqlQuery)}&format=json";

            var handler = new MockHttpMessageHandler(new Dictionary<string, HttpResponseMessage>
            {
                {
                    $"GET:{url}",
                    new HttpResponseMessage(HttpStatusCode.InternalServerError)
                }
            });

            var loggerMock = new Mock<ILogger<WikidataFrenchNameStrategy>>();
            var httpClient = new HttpClient(handler);
            var strategy = new WikidataFrenchNameStrategy(loggerMock.Object, httpClient);

            var result = strategy.MapName("ErrorCompound");
            result.Should().Be("ErrorCompound");
            
            // Check warning for status code
            loggerMock.Verify(
               x => x.Log(
                   LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Wikidata returned status code")),
                   null,
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once);
        }
    }

    /// <summary>
    /// Simple HTTP message handler for testing that returns predefined responses
    /// based on the request method and URL.
    /// </summary>
    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> _responses;
        private readonly List<string> _requestsMade = new List<string>();

        public IReadOnlyList<string> RequestsMade => _requestsMade;

        public MockHttpMessageHandler(Dictionary<string, HttpResponseMessage> responses)
        {
            _responses = responses;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var key = $"{request.Method}:{request.RequestUri.AbsoluteUri}";
            _requestsMade.Add(key);

            if (_responses.TryGetValue(key, out var response))
            {
                return Task.FromResult(response);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("No mock response configured for: " + key)
            });
        }
    }
}
