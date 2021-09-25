using FluentAssertions;
using MyMicroservices.Customers.DataAnnotations;
using Xunit;

namespace MyMicroservices.Customers.UnitTest.DataAnnotations
{
    public class ExpiryDateAttributeUnitTest
    {
        private readonly ExpiryDateAttribute _sut;
        public ExpiryDateAttributeUnitTest()
        {
            _sut = new ExpiryDateAttribute();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_IfMonthEqualsToZero()
        {
            //arrange
            string input = "00/9999";
            //act 
            var result = _sut.IsValid(input);
            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_IfMonthGreaterThanTwelve()
        {
            //arrange
            string input = "13/9999";
            //act 
            var result = _sut.IsValid(input);
            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_IfYearGreaterEqualsToZero()
        {
            //arrange
            string input = "01/0000";
            //act 
            var result = _sut.IsValid(input);
            //assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("01/9999")]
        [InlineData("02/9999")]
        [InlineData("03/9999")]
        [InlineData("04/9999")]
        [InlineData("05/9999")]
        [InlineData("06/9999")]
        [InlineData("07/9999")]
        [InlineData("08/9999")]
        [InlineData("09/9999")]
        [InlineData("10/9999")]
        [InlineData("11/9999")]
        [InlineData("12/9999")]
        public void IsValid_ShouldReturnTrue_IfInputIsValid(string input)
        {
            //arrange

            //act 
            var result = _sut.IsValid(input);
            //assert
            result.Should().BeTrue();
        }
    }
}
