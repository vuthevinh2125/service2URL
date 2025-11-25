// ShortenUrl.Tests/ShortCodeGeneratorTests.cs

using ShortenUrl.Services; // Tham chiếu đến dịch vụ cần kiểm tra

namespace ShortenUrl.Tests
{
    public class ShortCodeGeneratorTests
    {
        [Fact] // Annotation cho biết đây là một phương thức kiểm thử
        public void GenerateUniqueCode_ShouldReturnCorrectLength()
        {
            // Arrange
            var generator = new ShortCodeGenerator();
            var expectedLength = 7;

            // Act
            var code = generator.GenerateUniqueCode();

            // Assert
            Assert.Equal(expectedLength, code.Length);
        }

        [Fact]
        public void GenerateUniqueCode_ShouldOnlyContainValidCharacters()
        {
            // Arrange
            var generator = new ShortCodeGenerator();
            var validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // Act
            var code = generator.GenerateUniqueCode();

            // Assert
            // Kiểm tra xem tất cả các ký tự được tạo ra có nằm trong bộ ký tự hợp lệ không
            Assert.True(code.All(c => validChars.Contains(c)));
        }
    }
}