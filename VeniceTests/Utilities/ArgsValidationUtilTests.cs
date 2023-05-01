using System;
using VeniceDomain;
using Xunit;

namespace VeniceTests.Utilities
{
    public class ArgsValidationUtilTests
    {
        [Fact]
        public void InRangeMinimumExclusive()
        {
            string argumentName = "test_minimum_exclusive";

            Exception exception = Assert.Throws<ArgumentException>(() => ArgsValidationUtil.InRange(1, argumentName, minimumExclusive: 1));
            Assert.Contains(argumentName, exception.Message);

            exception = Record.Exception(() => ArgsValidationUtil.InRange(1.01M, argumentName, minimumExclusive: 1));
            Assert.Null(exception);
        }

        [Fact]
        public void InRangeMinimumInclusive()
        {
            string argumentName = "test_minimum_inclusive";

            Exception exception = Assert.Throws<ArgumentException>(() => ArgsValidationUtil.InRange(0.99M, argumentName, minimumInclusive: 1));
            Assert.Contains(argumentName, exception.Message);

            exception = Record.Exception(() => ArgsValidationUtil.InRange(1, argumentName, minimumInclusive: 1));
            Assert.Null(exception);
        }

        [Fact]
        public void InRangeMaximumExclusive()
        {
            string argumentName = "test_maximum_exclusive";

            Exception exception = Assert.Throws<ArgumentException>(() => ArgsValidationUtil.InRange(1, argumentName, maximumExclusive: 1));
            Assert.Contains(argumentName, exception.Message);

            exception = Record.Exception(() => ArgsValidationUtil.InRange(0.99M, argumentName, maximumExclusive: 1));
            Assert.Null(exception);
        }

        [Fact]
        public void InRangeMaximumInclusive()
        {
            string argumentName = "test_maximum_inclusive";

            Exception exception = Assert.Throws<ArgumentException>(() => ArgsValidationUtil.InRange(1.01M, argumentName, maximumInclusive: 1));
            Assert.Contains(argumentName, exception.Message);

            exception = Record.Exception(() => ArgsValidationUtil.InRange(1, argumentName, maximumInclusive: 1));
            Assert.Null(exception);
        }
    }
}
