using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HR.SharedKernel.Security
{
    public static class PassWordRegex
    {
        public static bool IsMatch(string Password)
        {
            Regex lettersOnly = new Regex("^[a-zA-Z]");
            if (!lettersOnly.IsMatch(Password))
            {
                return false;
            }
            var hasUpperChar = new Regex(@"[A-Z]+");
            if (!hasUpperChar.IsMatch(Password))
            {
                return false;
            }
            var hasMinimum8Chars = new Regex(@".{8,}");
            if (!hasMinimum8Chars.IsMatch(Password))
            {
                return false;
            }
            var hasNumber = new Regex(@"[0-9]+");

            if (!hasNumber.IsMatch(Password))
            {
                return false;
            }
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasSymbols.IsMatch(Password))
            {
                return false;
            }

            const int MIN_LENGTH = 8;
            const int MAX_LENGTH = 150;

            if (string.IsNullOrEmpty(Password)) throw new ArgumentNullException();

            bool meetsLengthRequirements = Password.Length >= MIN_LENGTH && Password.Length <= MAX_LENGTH;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            if (meetsLengthRequirements)
            {
                foreach (char c in Password)
                {
                    if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    else if (char.IsDigit(c)) hasDecimalDigit = true;
                }
            }

            bool isValid = meetsLengthRequirements
                        && hasUpperCaseLetter
                        && hasLowerCaseLetter
                        && hasDecimalDigit
                        ;
            return isValid;
        }
    }
}
