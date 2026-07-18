using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace HR.SharedKernel.Security;

/// <summary>
/// Service for encrypting and decrypting userId in JWT tokens to prevent information leakage
/// Uses AES-256-CBC encryption with a key derived from JWT configuration
/// </summary>
public class UserIdEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public UserIdEncryptionService(IConfiguration configuration)
    {
        // Derive encryption key from JWT key to avoid storing separate key
        var jwtKey = configuration.GetSection("Jwt:Key").Value ?? 
                     throw new InvalidOperationException("JWT:Key configuration is required");
        
        // Use SHA-256 to derive a 32-byte key from JWT key
        using (var sha256 = SHA256.Create())
        {
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(jwtKey + "_UserIdEncryption"));
        }

        // Use first 16 bytes of key hash as IV (Initialization Vector)
        // In production, consider using a random IV per encryption and storing it with the ciphertext
        using (var sha256 = SHA256.Create())
        {
            var ivHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(jwtKey + "_UserIdIV"));
            _iv = new byte[16];
            Array.Copy(ivHash, 0, _iv, 0, 16);
        }
    }

    /// <summary>
    /// Encrypts a userId to a base64 string for storage in JWT token
    /// </summary>
    public string EncryptUserId(long userId)
    {
        try
        {
            var userIdString = userId.ToString();
            var plainBytes = Encoding.UTF8.GetBytes(userIdString);

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    var encrypted = msEncrypt.ToArray();
                    return Convert.ToBase64String(encrypted);
                }
            }
        }
        catch (Exception ex)
        {
            throw new CryptographicException($"Failed to encrypt userId: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Decrypts an encrypted userId string from JWT token back to long
    /// Returns -1 if decryption fails
    /// </summary>
    public long DecryptUserId(string encryptedUserId)
    {
        if (string.IsNullOrWhiteSpace(encryptedUserId))
        {
            return -1;
        }

        try
        {
            var cipherBytes = Convert.FromBase64String(encryptedUserId);

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                using (var msDecrypt = new System.IO.MemoryStream(cipherBytes))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                {
                    var decryptedText = srDecrypt.ReadToEnd();
                    if (long.TryParse(decryptedText, out var userId))
                    {
                        return userId;
                    }
                }
            }
        }
        catch
        {
            // Return -1 on any decryption error (invalid format, wrong key, etc.)
        }

        return -1;
    }

    /// <summary>
    /// Tries to decrypt userId, returns false if decryption fails
    /// </summary>
    public bool TryDecryptUserId(string encryptedUserId, out long userId)
    {
        userId = DecryptUserId(encryptedUserId);
        return userId > 0;
    }
}

