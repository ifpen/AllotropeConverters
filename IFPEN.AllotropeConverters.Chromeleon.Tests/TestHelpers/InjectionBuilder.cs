using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Thermo.Chromeleon.Sdk.Interfaces.Data;
using Thermo.Chromeleon.Sdk.Interfaces.Data.InstrumentMethodScript;
using Thermo.Chromeleon.Sdk.Interfaces.Instruments.Symbols;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests.TestHelpers
{
    /// <summary>
    /// Builder for creating mock IInjection instances for testing.
    /// </summary>
    public class InjectionBuilder
    {
        private string _name = "TestInjection";
        private DateTimeOffset? _injectTime = DateTimeOffset.UtcNow;
        private double? _injectionVolume = 10.0;
        private string _instrumentMethodName = "DefaultMethod";
        private List<ISignal> _signals = new List<ISignal>();
        private ISymbol _rootSymbol;

        /// <summary>
        /// Sets the injection name.
        /// </summary>
        public InjectionBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Sets the injection time.
        /// </summary>
        public InjectionBuilder WithInjectTime(DateTimeOffset time)
        {
            _injectTime = time;
            return this;
        }

        /// <summary>
        /// Sets the injection time to null.
        /// </summary>
        public InjectionBuilder WithNullInjectTime()
        {
            _injectTime = null;
            return this;
        }

        /// <summary>
        /// Sets the injection volume.
        /// </summary>
        public InjectionBuilder WithInjectionVolume(double volume)
        {
            _injectionVolume = volume;
            return this;
        }

        /// <summary>
        /// Sets the injection volume to null.
        /// </summary>
        public InjectionBuilder WithNullInjectionVolume()
        {
            _injectionVolume = null;
            return this;
        }

        /// <summary>
        /// Sets the instrument method name.
        /// </summary>
        public InjectionBuilder WithInstrumentMethodName(string methodName)
        {
            _instrumentMethodName = methodName;
            return this;
        }

        /// <summary>
        /// Adds signals to the injection.
        /// </summary>
        public InjectionBuilder WithSignals(params ISignal[] signals)
        {
            _signals.AddRange(signals);
            return this;
        }

        /// <summary>
        /// Sets the root symbol via a mock InstrumentMethod.
        /// </summary>
        public InjectionBuilder WithRootSymbol(ISymbol symbol)
        {
            _rootSymbol = symbol;
            return this;
        }

        /// <summary>
        /// Builds and returns a configured mock IInjection.
        /// </summary>
        public IInjection Build()
        {
            var injectionMock = new Mock<IInjection>();
            
            injectionMock.Setup(i => i.Name).Returns(_name);
            injectionMock.Setup(i => i.InjectTime).Returns(_injectTime);
            injectionMock.Setup(i => i.InstrumentMethodName).Returns(MockHelpers.CreateIStringValue(_instrumentMethodName));
            injectionMock.Setup(i => i.InjectionVolume).Returns(MockHelpers.CreateINumericValue(_injectionVolume));
            injectionMock.Setup(i => i.Comment).Returns(MockHelpers.CreateIStringValue("DefaultComment"));

            // Mock Signals collection
            var signalsMock = new Mock<Thermo.Chromeleon.Sdk.Interfaces.Common.Collections.IReadOnlyList<ISignal>>();
            signalsMock.As<IEnumerable<ISignal>>().Setup(s => s.GetEnumerator())
                .Returns(() => _signals.GetEnumerator());
            signalsMock.As<IEnumerable>().Setup(s => s.GetEnumerator())
                .Returns(() => _signals.GetEnumerator());
            injectionMock.Setup(i => i.Signals).Returns(signalsMock.Object);

            // Mock InstrumentMethod with Script and RootSymbol
            if (_rootSymbol != null)
            {
                var scriptMock = new Mock<IScript>();
                scriptMock.Setup(s => s.RootSymbol).Returns(_rootSymbol);

                var instrumentMethodMock = new Mock<IInstrumentMethod>();
                instrumentMethodMock.Setup(m => m.Script).Returns(scriptMock.Object);

                injectionMock.Setup(i => i.InstrumentMethod).Returns(instrumentMethodMock.Object);
            }

            return injectionMock.Object;
        }
    }
}
