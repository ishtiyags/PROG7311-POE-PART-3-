using System;
using Xunit;
using TechMoveCRM.Services;

namespace TechMoveCRM.Tests
{
    public class CurrencyServiceTests
    {
        // We only test the pure math — no HTTP calls needed.
        // We use a concrete instance since ConvertUsdToZar has no dependencies.
        private readonly CurrencyService _service;

        public CurrencyServiceTests()
        {
            // Pass nulls — ConvertUsdToZar doesn't use HttpClient or config
            _service = new CurrencyService(null, null);
        }

        [Fact]
        public void ConvertUsdToZar_WithValidInputs_ReturnsCorrectAmount()
        {
            // Arrange
            decimal usdAmount = 100m;
            decimal rate = 18.50m;

            // Act
            decimal result = _service.ConvertUsdToZar(usdAmount, rate);

            // Assert
            Assert.Equal(1850.00m, result);
        }

        [Fact]
        public void ConvertUsdToZar_WithZeroUsd_ReturnsZero()
        {
            // Arrange
            decimal usdAmount = 0m;
            decimal rate = 18.50m;

            // Act
            decimal result = _service.ConvertUsdToZar(usdAmount, rate);

            // Assert — Zero USD should give zero ZAR
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ConvertUsdToZar_WithFractionalUsd_RoundsToTwoDecimalPlaces()
        {
            // Arrange
            decimal usdAmount = 1m;
            decimal rate = 18.333m;  // Should round to 18.33

            // Act
            decimal result = _service.ConvertUsdToZar(usdAmount, rate);

            // Assert
            Assert.Equal(18.33m, result);
        }

        [Fact]
        public void ConvertUsdToZar_WithNegativeUsd_ThrowsArgumentException()
        {
            // Arrange
            decimal usdAmount = -50m;
            decimal rate = 18.50m;

            // Act & Assert — negative USD is invalid
            Assert.Throws<ArgumentException>(() =>
                _service.ConvertUsdToZar(usdAmount, rate));
        }

        [Fact]
        public void ConvertUsdToZar_WithZeroRate_ThrowsArgumentException()
        {
            // Arrange — zero rate would cause division-by-zero or nonsense result
            decimal usdAmount = 100m;
            decimal rate = 0m;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _service.ConvertUsdToZar(usdAmount, rate));
        }

        [Fact]
        public void ConvertUsdToZar_WithNegativeRate_ThrowsArgumentException()
        {
            // Arrange
            decimal usdAmount = 100m;
            decimal rate = -5m;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _service.ConvertUsdToZar(usdAmount, rate));
        }

        [Theory]
        [InlineData(10, 18.50, 185.00)]
        [InlineData(250, 19.00, 4750.00)]
        [InlineData(0.01, 18.50, 0.19)]   // Edge: tiny amount
        [InlineData(1000000, 18.50, 18500000.00)] // Edge: large amount
        public void ConvertUsdToZar_MultipleScenarios_AreCorrect(
            decimal usd, decimal rate, decimal expected)
        {
            var result = _service.ConvertUsdToZar(usd, rate);
            Assert.Equal(expected, result);
        }
    }
}