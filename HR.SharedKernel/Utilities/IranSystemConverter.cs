using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HR.SharedKernel.Converters
{
    public static class IranSystemConverter
    {
        private static readonly Encoding _encoding1256 = Encoding.GetEncoding("windows-1256");

        // Mapping from IranSystem byte codes to Windows-1256 (Unicode) byte codes
        private static readonly Dictionary<byte, byte> _iranSystemToWindows1256 = new Dictionary<byte, byte>
        {
            // Numbers
            {128, 48}, // 0
            {129, 49}, // 1
            {130, 50}, // 2
            {131, 51}, // 3
            {132, 52}, // 4
            {133, 53}, // 5
            {134, 54}, // 6
            {135, 55}, // 7
            {136, 56}, // 8
            {137, 57}, // 9
            
            // Punctuation
            {138, 161}, // ،
            {139, 220}, // -
            {140, 191}, // ؟
            
            // Letters (various forms)
            {141, 194}, // آ
            {142, 196}, 
            {143, 193},
            {144, 195}, // ا
            {145, 195}, // ا
            {146, 200}, // ب
            {147, 200},
            {148, 129},
            {149, 129},
            {150, 202}, // ت
            {151, 202},
            {152, 203}, // ث
            {153, 203},
            {154, 204}, // ج
            {155, 204},
            {156, 141},
            {157, 141},
            {158, 205}, // ح
            {159, 205},
            {160, 206}, // خ
            {161, 206},
            {162, 207}, // د
            {163, 208}, // ذ
            {164, 209}, // ر
            {165, 210}, // ز
            {166, 142},
            {167, 211}, // س
            {168, 211},
            {169, 212}, // ش
            {170, 212},
            {171, 213}, // ص
            {172, 213},
            {173, 214}, // ض
            {174, 214},
            {175, 216}, // ط
            {224, 217}, // ظ
            {225, 218}, // ع
            {226, 218},
            {227, 218},
            {228, 218},
            {229, 219}, // غ
            {230, 219},
            {231, 219},
            {232, 219},
            {233, 221}, // ف
            {234, 221},
            {235, 222}, // ق
            {236, 222},
            {237, 223}, // ك
            {238, 223},
            {239, 144},
            {240, 144},
            {241, 225}, // ل
            {242, 225}, // لا
            {243, 225},
            {244, 227}, // م
            {245, 227},
            {246, 228}, // ن
            {247, 228},
            {248, 230},
            {249, 229}, // ه
            {250, 229},
            {251, 229},
            {252, 236}, // ى
            {253, 237}, // ي
            {254, 236},
        };

        /// <summary>
        /// Convert IranSystem encoded text to Unicode (Windows-1256)
        /// </summary>
        public static string FromIranSystem(byte[] iranSystemBytes)
        {
            if (iranSystemBytes == null || iranSystemBytes.Length == 0)
                return string.Empty;

            // Reverse the byte array first (IranSystem is RTL)
            var reversedBytes = iranSystemBytes.Reverse().ToArray();
            
            // Convert IranSystem bytes to Windows-1256 bytes
            var windows1256Bytes = new List<byte>();
            
            foreach (var iranByte in reversedBytes)
            {
                if (_iranSystemToWindows1256.TryGetValue(iranByte, out byte windowsByte))
                {
                    windows1256Bytes.Add(windowsByte);
                }
                else if (iranByte == 32 || iranByte == 0 || (iranByte >= 32 && iranByte < 128)) // Space or ASCII
                {
                    windows1256Bytes.Add(iranByte);
                }
                else
                {
                    // Unknown character, keep as is
                    windows1256Bytes.Add(iranByte);
                }
            }

            // Convert Windows-1256 bytes to Unicode string
            var result = _encoding1256.GetString(windows1256Bytes.ToArray()).Trim();
            
            return result;
        }

        /// <summary>
        /// Convert IranSystem string to Unicode
        /// </summary>
        public static string FromIranSystemString(string iranSystemText)
        {
            if (string.IsNullOrWhiteSpace(iranSystemText))
                return iranSystemText ?? string.Empty;

            var bytes = _encoding1256.GetBytes(iranSystemText);
            return FromIranSystem(bytes);
        }
    }
}

