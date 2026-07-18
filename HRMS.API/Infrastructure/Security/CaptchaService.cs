using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;

namespace HRMS.API.Infrastructure.Security;

public class CaptchaService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CaptchaService> _logger;
    private readonly CaptchaOptions _options;
    private static readonly char[] _chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789".ToCharArray();
    private const int CaptchaLength = 5;
    private static readonly TimeSpan Expiration = TimeSpan.FromMinutes(2);

    public CaptchaService(IMemoryCache cache, ILogger<CaptchaService> logger, IOptions<CaptchaOptions> options)
    {
        _cache = cache;
        _logger = logger;
        _options = options.Value;
    }

    /// <summary>
    /// Track a failed login attempt for a specific IP address
    /// </summary>
    public void TrackFailedAttempt(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) return;

        var key = $"failed_attempts:{ipAddress}";
        
        if (_cache.TryGetValue<int>(key, out var attempts))
        {
            attempts++;
        }
        else
        {
            attempts = 1;
        }

        var window = TimeSpan.FromMinutes(_options.FailedAttemptsWindowMinutes);
        _cache.Set(key, attempts, window);
        _logger.LogWarning("Failed login attempt tracked for IP {IP}. Total attempts: {Attempts}", ipAddress, attempts);
    }

    /// <summary>
    /// Check if captcha is required for a specific IP address based on failed attempts
    /// </summary>
    public bool IsCaptchaRequired(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) return false;

        var key = $"failed_attempts:{ipAddress}";
        
        if (_cache.TryGetValue<int>(key, out var attempts))
        {
            return attempts >= _options.MaxFailedAttempts;
        }

        return false;
    }
    
    /// <summary>
    /// Get whether captcha is enabled in development mode
    /// </summary>
    public bool IsEnabledInDevelopment => _options.EnableInDevelopment;

    /// <summary>
    /// Reset failed attempts counter for a specific IP address (called on successful login)
    /// </summary>
    public void ResetFailedAttempts(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) return;

        var key = $"failed_attempts:{ipAddress}";
        _cache.Remove(key);
        _logger.LogInformation("Failed attempts reset for IP {IP}", ipAddress);
    }

    public (string Id, byte[] ImageBytes, string Text) Generate()
    {
        string text = GenerateText(CaptchaLength);
        string id = Guid.NewGuid().ToString("N");

        // store normalized solution in cache
        _cache.Set(CacheKey(id), Normalize(text), Expiration);

        var bytes = RenderImage(text);
        return (id, bytes, text);
    }

    public bool Validate(string? id, string? code)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(code)) return false;
            var key = CacheKey(id);
            if (!_cache.TryGetValue<string>(key, out var expected))
            {
                return false;
            }
            // remove on read (one-time)
            _cache.Remove(key);
            return string.Equals(Normalize(code), expected, StringComparison.Ordinal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Captcha validation error");
            return false;
        }
    }

    private static string CacheKey(string id) => $"captcha:{id}";

    private static string GenerateText(int len)
    {
        var bytes = new byte[len];
        RandomNumberGenerator.Fill(bytes);
        var sb = new StringBuilder(len);
        for (int i = 0; i < len; i++)
        {
            sb.Append(_chars[bytes[i] % _chars.Length]);
        }
        return sb.ToString();
    }

    private static string Normalize(string value) => value.Replace(" ", string.Empty).Trim().ToLowerInvariant();

    private static byte[] RenderImage(string text)
    {
        const int width = 180;
        const int height = 56;
        using var bmp = new Bitmap(width, height);
        using var gfx = Graphics.FromImage(bmp);
        gfx.SmoothingMode = SmoothingMode.AntiAlias;
        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
        gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

        // background
        gfx.Clear(Color.FromArgb(246, 247, 249));

        var rand = new Random();

        // noise lines
        for (int i = 0; i < 6; i++)
        {
            using var pen = new Pen(Color.FromArgb(rand.Next(120, 200), rand.Next(120, 200), rand.Next(120, 200)));
            gfx.DrawLine(pen, rand.Next(width), rand.Next(height), rand.Next(width), rand.Next(height));
        }

        // dots
        for (int i = 0; i < 40; i++)
        {
            using var brush = new SolidBrush(Color.FromArgb(rand.Next(160, 220), rand.Next(160, 220), rand.Next(160, 220)));
            gfx.FillEllipse(brush, rand.Next(width), rand.Next(height), 2, 2);
        }

        // text
        using var font = new Font("Arial", 28, FontStyle.Bold);
        var charSpacing = 28;
        for (int i = 0; i < text.Length; i++)
        {
            var ch = text[i].ToString();
            var x = 20 + i * charSpacing;
            var y = height / 2;
            var angle = (float)((rand.NextDouble() - 0.5) * 30.0); // -15..+15 degrees
            gfx.ResetTransform();
            gfx.TranslateTransform(x, y);
            gfx.RotateTransform(angle);
            using var brush = new SolidBrush(Color.FromArgb(rand.Next(60, 120), rand.Next(60, 120), rand.Next(60, 120)));
            var size = gfx.MeasureString(ch, font);
            gfx.DrawString(ch, font, brush, -size.Width / 2, -size.Height / 2);
        }
        gfx.ResetTransform();

        using var ms = new MemoryStream();
        // Save bitmap to memory stream as PNG
        // In .NET Core/.NET 5+, ImageFormat.Png should work without explicit encoder
        bmp.Save(ms, ImageFormat.Png);
        ms.Position = 0;
        return ms.ToArray();
    }
}


