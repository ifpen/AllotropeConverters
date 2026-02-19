using System;
using System.Collections.Generic;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;

namespace Ifpen.AllotropeConverters.Chromeleon.PeakNameStrategies
{
    /// <summary>
    /// Decorator that adds in-memory caching to any <see cref="IPeakNameMappingStrategy"/>.
    /// The cache lives for the duration of the session (application lifetime).
    /// </summary>
    public class MemoryCachePeakNameStrategy : IPeakNameMappingStrategy
    {
        private readonly IPeakNameMappingStrategy _inner;
        private readonly Dictionary<string, string> _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCachePeakNameStrategy"/> class.
        /// </summary>
        /// <param name="inner">The inner strategy to delegate to on cache miss.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is null.</exception>
        public MemoryCachePeakNameStrategy(IPeakNameMappingStrategy inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public string MapName(string originalName)
        {
            if (originalName == null) return _inner.MapName(originalName);

            if (_cache.TryGetValue(originalName, out string cached))
            {
                return cached;
            }

            string mapped = _inner.MapName(originalName);
            _cache[originalName] = mapped;
            return mapped;
        }
    }
}
