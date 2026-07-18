using System.Security.Cryptography;
using System.Text;

namespace HRMS.API.Infrastructure.Security;

/// <summary>
/// Provides a per-process RSA key pair used to encrypt login passwords on the client
/// and decrypt them on the server. Generates a new key pair on application start.
/// </summary>
public sealed class RsaKeyService
{
    private readonly RSA _rsa;
    private readonly string _keyId;

    public RsaKeyService()
    {
        _rsa = RSA.Create(2048);
        _keyId = Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// Returns the current key id and the public key in PEM (SPKI) format
    /// suitable for WebCrypto.importKey('spki', ...).
    /// </summary>
    public (string keyId, string publicKeyPem) GetCurrentPublicKey()
    {
        var spki = _rsa.ExportSubjectPublicKeyInfo();
        var base64 = Convert.ToBase64String(spki);
        var sb = new StringBuilder();
        sb.AppendLine("-----BEGIN PUBLIC KEY-----");
        for (int i = 0; i < base64.Length; i += 64)
        {
            var len = Math.Min(64, base64.Length - i);
            sb.AppendLine(base64.Substring(i, len));
        }
        sb.AppendLine("-----END PUBLIC KEY-----");
        return (_keyId, sb.ToString());
    }

    /// <summary>
    /// Attempts to decrypt the given Base64 ciphertext that was encrypted
    /// using RSA-OAEP with SHA-256. Returns false if decryption fails.
    /// Extracts password from format: password|timestamp|nonce
    /// </summary>
    public bool TryDecrypt(string keyId, string base64CipherText, out string? plaintext)
    {
        plaintext = null;
        if (!string.Equals(keyId, _keyId, StringComparison.Ordinal))
        {
            return false;
        }
        try
        {
            var cipherBytes = Convert.FromBase64String(base64CipherText);
            var plainBytes = _rsa.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);
            var decryptedText = Encoding.UTF8.GetString(plainBytes);
            
            // Extract password from format: password|timestamp|nonce
            // The client sends password with timestamp and nonce for one-time use encryption
            var parts = decryptedText.Split('|', 3);
            if (parts.Length >= 1 && !string.IsNullOrEmpty(parts[0]))
            {
                plaintext = parts[0]; // Return only the password part
                return true;
            }
            
            // Fallback: if format doesn't match (backward compatibility), return as-is
            plaintext = decryptedText;
            return true;
        }
        catch
        {
            return false;
        }
    }
}


