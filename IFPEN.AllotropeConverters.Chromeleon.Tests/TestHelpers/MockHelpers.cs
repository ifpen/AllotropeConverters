using System;
using Moq;
using Thermo.Chromeleon.Sdk.Interfaces.Types;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests.TestHelpers
{
    /// <summary>
    /// Provides helper methods for creating mocks of Chromeleon SDK types.
    /// These helpers enable unit testing without a Chromeleon client installed.
    /// </summary>
    public static class MockHelpers
    {
        /// <summary>
        /// Creates a mock of IStringValue with the specified value.
        /// </summary>
        /// <param name="value">The string value to return.</param>
        /// <returns>A configured mock of IStringValue.</returns>
        public static IStringValue CreateIStringValue(string value)
        {
            var mock = new Mock<IStringValue>();
            mock.Setup(x => x.Value).Returns(value);
            return mock.Object;
        }

        /// <summary>
        /// Creates a mock of INumericValue with the specified value.
        /// </summary>
        /// <param name="value">The numeric value to return.</param>
        /// <returns>A configured mock of INumericValue.</returns>
        public static INumericValue CreateINumericValue(double? value)
        {
            var mock = new Mock<INumericValue>();
            mock.Setup(x => x.Value).Returns(value);
            return mock.Object;
        }

        /// <summary>
        /// Creates a Nullable DateTimeOffset, which is the type used by IInjection.InjectTime.
        /// </summary>
        /// <param name="value">The DateTimeOffset value.</param>
        /// <returns>A nullable DateTimeOffset.</returns>
        public static DateTimeOffset? CreateNullableDateTimeOffset(DateTimeOffset value)
        {
            return value;
        }

        /// <summary>
        /// Creates a null Nullable DateTimeOffset.
        /// </summary>
        /// <returns>A null DateTimeOffset.</returns>
        public static DateTimeOffset? CreateNullDateTimeOffset()
        {
            return null;
        }
    }
}
