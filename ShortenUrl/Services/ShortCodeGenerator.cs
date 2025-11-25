using Xunit;
namespace ShortenUrl.Services
{
    public class ShortCodeGenerator
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int CodeLength = 7;

        private readonly Random _random = new Random();

        public string GenerateUniqueCode()
        {
            var code = new char[CodeLength];
            for (int i = 0; i < CodeLength; i++)
            {
                code[i] = Alphabet[_random.Next(Alphabet.Length)];
            }
            return new string(code);
        }
    }
}