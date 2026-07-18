using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HR.SharedKernel.Security;

/// <summary>
/// سرویس Generic برای رمزنگاری و رمزگشایی فایل‌ها با استفاده از AES-256
/// </summary>
public static class FileEncryptionService
{
    private const int KeySize = 256;
    private const int IvSize = 128;
    private const int SaltSize = 32;
    private const int Iterations = 100000; // PBKDF2 iterations

    /// <summary>
    /// رمزنگاری فایل با استفاده از رمز عبور
    /// </summary>
    /// <param name="fileBytes">بایت‌های فایل برای رمزنگاری</param>
    /// <param name="password">رمز عبور برای رمزنگاری</param>
    /// <returns>بایت‌های فایل رمزنگاری شده</returns>
    public static byte[] EncryptFile(byte[] fileBytes, string password)
    {
        if (fileBytes == null || fileBytes.Length == 0)
            throw new ArgumentException("فایل نمی‌تواند خالی باشد", nameof(fileBytes));
        
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("رمز عبور نمی‌تواند خالی باشد", nameof(password));

        // Generate random salt
        var salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Derive key from password using PBKDF2
        var key = DeriveKeyFromPassword(password, salt);

        // Generate random IV
        var iv = new byte[IvSize / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(iv);
        }

        // Encrypt the file
        byte[] encryptedBytes;
        using (var aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.BlockSize = IvSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using (var encryptor = aes.CreateEncryptor())
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(fileBytes, 0, fileBytes.Length);
                }
                encryptedBytes = msEncrypt.ToArray();
            }
        }

        // Combine salt + IV + encrypted data
        var result = new byte[SaltSize + IvSize / 8 + encryptedBytes.Length];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(iv, 0, result, SaltSize, IvSize / 8);
        Buffer.BlockCopy(encryptedBytes, 0, result, SaltSize + IvSize / 8, encryptedBytes.Length);

        return result;
    }

    /// <summary>
    /// رمزگشایی فایل با استفاده از رمز عبور
    /// </summary>
    /// <param name="encryptedFileBytes">بایت‌های فایل رمزنگاری شده</param>
    /// <param name="password">رمز عبور برای رمزگشایی</param>
    /// <returns>بایت‌های فایل رمزگشایی شده</returns>
    public static byte[] DecryptFile(byte[] encryptedFileBytes, string password)
    {
        if (encryptedFileBytes == null || encryptedFileBytes.Length == 0)
            throw new ArgumentException("فایل رمزنگاری شده نمی‌تواند خالی باشد", nameof(encryptedFileBytes));
        
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("رمز عبور نمی‌تواند خالی باشد", nameof(password));

        // Minimum size check: salt + IV + at least some encrypted data
        var minSize = SaltSize + IvSize / 8 + 16; // 16 is minimum block size for AES
        if (encryptedFileBytes.Length < minSize)
            throw new ArgumentException("فایل رمزنگاری شده نامعتبر است", nameof(encryptedFileBytes));

        // Extract salt, IV, and encrypted data
        var salt = new byte[SaltSize];
        var iv = new byte[IvSize / 8];
        var encryptedData = new byte[encryptedFileBytes.Length - SaltSize - IvSize / 8];

        Buffer.BlockCopy(encryptedFileBytes, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(encryptedFileBytes, SaltSize, iv, 0, IvSize / 8);
        Buffer.BlockCopy(encryptedFileBytes, SaltSize + IvSize / 8, encryptedData, 0, encryptedData.Length);

        // Derive key from password using PBKDF2
        var key = DeriveKeyFromPassword(password, salt);

        // Decrypt the file
        byte[] decryptedBytes;
        using (var aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.BlockSize = IvSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor())
            using (var msDecrypt = new MemoryStream(encryptedData))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var msResult = new MemoryStream())
            {
                csDecrypt.CopyTo(msResult);
                decryptedBytes = msResult.ToArray();
            }
        }

        return decryptedBytes;
    }

    /// <summary>
    /// استخراج کلید از رمز عبور با استفاده از PBKDF2
    /// </summary>
    private static byte[] DeriveKeyFromPassword(string password, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(KeySize / 8); // 32 bytes for AES-256
        }
    }
}

