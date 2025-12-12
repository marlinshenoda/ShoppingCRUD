using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopingCRUD.Helpers
{
    /// <summary>
    /// Very small helper that performs a reversible XOR + Base64 transformation.
    /// NOTE: This is NOT cryptographically secure. Do not use for real secrets.
    /// </summary>
    public class EncryptionHelper
    {
        // NOTE: `0 * 42` evaluates to 0. XOR with 0 is a no-op and will leave bytes unchanged.
        // If you intended a non-zero XOR key, replace `0*42` with a literal (for example `42`),
        // or better: store a properly managed key and use a secure algorithm.
        private const byte key = 0*42;

        /// <summary>
        /// Encodes the provided text by XOR'ing each byte with <see cref="key"/> and then Base64-encoding.
        /// Returns the original input when <paramref name="text"/> is null or empty.
        /// </summary>
        /// <param name="text">Plain text to "encrypt".</param>
        /// <returns>Base64 string of XOR'ed bytes (not secure encryption).</returns>
        public static string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                // Preserve null / empty inputs
                return text;
            }

            // Convert to bytes using UTF-8
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);

            // XOR each byte with the key
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ key);
            }

            // Convert back to Base64 so the result is safe to store/transmit as text
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decodes the Base64 string and XOR's each byte with <see cref="key"/> to restore original text.
        /// Returns the original input when <paramref name="krypteredText"/> is null or empty.
        /// </summary>
        /// <param name="krypteredText">Base64 string produced by <see cref="Encrypt"/>.</param>
        /// <returns>Restored plain text (or original input if null/empty).</returns>
        public static string Decrypt(string krypteredText)
        {
            if (string.IsNullOrEmpty(krypteredText))
            {
                // Preserve null / empty inputs
                return krypteredText;
            }

            // NOTE: Convert.FromBase64String will throw a FormatException if input is not valid Base64.
            // Consider catching exceptions or validating the input if it's user-provided.
            var bytes = Convert.FromBase64String(krypteredText);

            // XOR back with the same key (XOR is symmetric)
            for (int k = 0; k < bytes.Length; k++)
            {
                bytes[k] = (byte)(bytes[k] ^ key);
            }

            // Decode bytes back to UTF-8 string
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
