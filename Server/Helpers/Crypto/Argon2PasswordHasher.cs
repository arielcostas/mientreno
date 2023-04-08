using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace Mientreno.Server.Helpers.Crypto;

public class Argon2PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    public string HashPassword(TUser _, string password)
    {
        var salt = GenerateSalt();
        var pass = GenerateHashBytes(
            Encoding.UTF8.GetBytes(password),
            salt
        );

        var fullHash = new byte[32 + 16];
        Buffer.BlockCopy(pass, 0, fullHash, 0, 32);
        Buffer.BlockCopy(salt, 0, fullHash, 32, 16);
        return Convert.ToBase64String(fullHash);
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser _, string hashedPassword, string providedPassword)
    {
        var bytes = Convert.FromBase64String(hashedPassword);
        if (bytes.Length != 48)
        {
            return PasswordVerificationResult.Failed;
        }

        var salt = new byte[16];
        var providedHash = new byte[32];
        Buffer.BlockCopy(bytes, 32, salt, 0, 16);
        Buffer.BlockCopy(bytes, 0, providedHash, 0, 32);

        // Hash the provided password and verify it
        var hash = GenerateHashBytes(
            Encoding.UTF8.GetBytes(providedPassword),
            salt
        );

        return CryptographicOperations.FixedTimeEquals(hash, providedHash)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }

    private static byte[] GenerateSalt()
    {
        var buf = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buf);
        return buf;
    }

    private static byte[] GenerateHashBytes(byte[] password, byte[] salt)
    {
        var a2 = new Argon2id(password)
        {
            Salt = salt,
            DegreeOfParallelism = 4,
            MemorySize = 1024 * 1024,
            Iterations = 3
        };
        return a2.GetBytes(32);
    }
}