

using System.Security.Cryptography;

namespace SchoolPilot.Infrastructure.Helpers
{

    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 100000;
        private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;
        private const char Delimiter = ';';

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty or whitespace.", nameof(password));
            }

            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                _hashAlgorithm,
                KeySize);

            return string.Join(Delimiter,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                throw new ArgumentException("Hashed password cannot be empty or whitespace.", nameof(hashedPassword));
            }

            if (string.IsNullOrWhiteSpace(providedPassword))
            {
                return false;
            }

            var elements = hashedPassword.Split(Delimiter);
            if (elements.Length != 2)
            {
                return false;
            }

            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);

            var hashInput = Rfc2898DeriveBytes.Pbkdf2(
                providedPassword,
                salt,
                Iterations,
                _hashAlgorithm,
                KeySize);

            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }
    }
}
