using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace vocabversus_engine.Utility
{
    public static class PasswordGenerator
    {
        private static string GeneratePassword(string password, byte[] salt)
        {
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashedPassword;
        }

        /// <summary>
        /// Creates hashed & salted password
        /// </summary>
        /// <param name="password">password input to hash</param>
        /// <param name="salt">salt used for hash</param>
        /// <returns>hashed password</returns>
        public static string GeneratePassword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(128 / 8);

            return GeneratePassword(password, salt);
        }

        /// <summary>
        /// Check given hashedpassword against input password
        /// </summary>
        /// <param name="hashedPassword">password to check against</param>
        /// <param name="salt">salt used for hashed password</param>
        /// <param name="password">password to be checked</param>
        /// <returns>true if passwords match, otherwise false</returns>
        public static bool VerifyPassword(string hashedPassword, byte[] salt, string password)
        {
            return hashedPassword == GeneratePassword(password, salt);
        }
    }
}
