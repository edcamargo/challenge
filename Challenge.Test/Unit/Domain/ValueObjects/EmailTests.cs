using Domain.ValueObjects;

namespace Tests.Domain.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Constructor_SetsEndereco()
        {
            var value = "test@example.com";
            var email = new Email(value);
            Assert.Equal(value, email.Endereco);
        }

        [Theory]
        [InlineData("a@b.com")]
        [InlineData("user.name+tag+sorting@example.com")]
        [InlineData("user@sub.example.co.uk")]
        public void IsValid_ReturnsTrue_ForValidEmails(string input)
        {
            Assert.True(Email.IsValid(input));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("plainaddress")]
        [InlineData("@missinglocal.com")]
        [InlineData("user@.com")]
        [InlineData("user@com")]
        [InlineData("user@@example.com")]
        public void IsValid_ReturnsFalse_ForInvalidEmails(string? input)
        {
            Assert.False(Email.IsValid(input));
        }
    }
}
