using System;
using System.Globalization;
using System.Linq;

namespace Utility
{
    /// <summary>
    ///     Utility Class for String
    /// </summary>
    public static class StringUtil
    {
        #region Static Fields

        private static readonly string[] SizeSuffixes = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        #endregion

        #region Public Methods and Operators

        public static string BytesToShortString(long value, int decimalPlaces = 1, bool integerForBytes = true)
        {
            if (decimalPlaces < 0)
                throw new ArgumentOutOfRangeException("decimalPlaces");

            if (value < 0)
                return "-" + BytesToShortString(-value);

            if (integerForBytes && value < 1000)
                return value + " bytes";

            if (value == 0)
                return string.Format("{0:n" + decimalPlaces + "} bytes", 0);

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            var mag = (int) Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag)
            // [i.e. the number of bytes in the unit corresponding to mag]
            var adjustedSize = (decimal) value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        /// <summary>
        ///     Generates a random string with a specified amount of characters
        /// </summary>
        public static string generateRandomString(int length = 32)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#$%&=_-";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        ///     Gets the formatted currency string according to ISO 4217 currency code.
        /// </summary>
        public static string getFormattedCurrencyString(decimal price, string isoCurrencyCode)
        {
            string res;
            var priceString = price.ToString("N", CultureInfo.CurrentCulture);
            if (string.CompareOrdinal(isoCurrencyCode, "USD") == 0)
                res = string.Format("${0}", priceString);
            else if (string.CompareOrdinal(isoCurrencyCode, "CAD") == 0)
                res = string.Format("${0} CAD", priceString);
            else
                res = string.Format("{0} {1}", priceString, isoCurrencyCode);
            return res;
        }

        #endregion
    }
}