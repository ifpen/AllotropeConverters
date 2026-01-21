using System;
using System.Collections.Generic;
using Moq;
using Thermo.Chromeleon.Sdk.Interfaces.Data;
using Thermo.Chromeleon.Sdk.Interfaces.RawData;
using Thermo.Chromeleon.Sdk.Interfaces.RawData.Collections;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests.TestHelpers
{
    /// <summary>
    /// Builder for creating mock ISignal instances for testing.
    /// </summary>
    public class SignalBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "DefaultSignal";
        private string _detectorDevice = "DefaultDetector";
        private string _user = "TestUser";
        private string _unit = "mAU";
        private string _channelName = "DefaultChannel";
        private IInjection _injection;

        /// <summary>
        /// Sets the signal ID.
        /// </summary>
        public SignalBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Sets the signal name.
        /// </summary>
        public SignalBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Sets the detector device name.
        /// </summary>
        public SignalBuilder WithDetector(string detector)
        {
            _detectorDevice = detector;
            return this;
        }

        /// <summary>
        /// Sets the user who created the signal.
        /// </summary>
        public SignalBuilder WithUser(string user)
        {
            _user = user;
            return this;
        }

        /// <summary>
        /// Sets the signal axis unit.
        /// </summary>
        public SignalBuilder WithUnit(string unit)
        {
            _unit = unit;
            return this;
        }

        /// <summary>
        /// Sets the channel name.
        /// </summary>
        public SignalBuilder WithChannelName(string channelName)
        {
            _channelName = channelName;
            return this;
        }

        /// <summary>
        /// Sets the parent injection for this signal.
        /// </summary>
        public SignalBuilder WithInjection(IInjection injection)
        {
            _injection = injection;
            return this;
        }

        /// <summary>
        /// Builds and returns a configured mock ISignal.
        /// </summary>
        public ISignal Build()
        {
            var signalMock = new Mock<ISignal>();
            signalMock.Setup(s => s.Id).Returns(_id);
            signalMock.Setup(s => s.Name).Returns(_name);

            // Mock Metadata
            var metadataMock = new Mock<ISignalMetadata>();
            metadataMock.Setup(m => m.DetectorDevice).Returns(_detectorDevice);
            metadataMock.Setup(m => m.User).Returns(_user);
            metadataMock.Setup(m => m.ChannelName).Returns(_channelName);

            // Mock SignalAxis
            var signalAxisMock = new Mock<IAxis>();
            signalAxisMock.Setup(a => a.Unit).Returns(_unit);
            metadataMock.Setup(m => m.SignalAxis).Returns(signalAxisMock.Object);

            // Mock TimeAxis
            var timeAxisMock = new Mock<IAxis>();
            timeAxisMock.Setup(a => a.Unit).Returns("min"); // Default to minutes
            metadataMock.Setup(m => m.TimeAxis).Returns(timeAxisMock.Object);

            signalMock.Setup(s => s.Metadata).Returns(metadataMock.Object);

            // Mock DataPoints
            var dataPointsMock = new Mock<IDataPointList>();
            dataPointsMock.Setup(d => d.Count).Returns(0);
            dataPointsMock.Setup(d => d.GetEnumerator()).Returns(new List<IDataPoint>().GetEnumerator());
            signalMock.Setup(s => s.DataPoints).Returns(dataPointsMock.Object);

            if (_injection != null)
            {
                signalMock.Setup(s => s.Injection).Returns(_injection);
            }

            return signalMock.Object;
        }
    }
}
