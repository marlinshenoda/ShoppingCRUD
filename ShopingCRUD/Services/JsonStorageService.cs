using ShopingCRUD.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShopingCRUD.Services
{
    /// <summary>
    /// Simple JSON-backed storage service for a list of T.
    /// - Loads and saves a JSON file at the provided path.
    /// - Uses reflection to optionally encrypt/decrypt an "Email" property on stored objects.
    /// 
    /// Notes:
    /// - Reflection is used for a convenience hook for an Email property; it's neither type-safe nor high-performance.
    /// - This class mutates the input objects when encrypting (see <see cref="SaveAsync"/>).
    /// - For real secrets use <see cref="System.Security.Cryptography"/> or OS-provided protection.
    /// </summary>
    public class JsonStorageService<T>
    {
        private readonly string _filePath;

        /// <summary>
        /// Create a new storage service bound to a file path.
        /// </summary>
        /// <param name="filePath">Full path to the JSON file used for persistence.</param>
        public JsonStorageService(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Load all objects from the JSON file.
        /// If the file does not exist an empty list is returned.
        /// If the stored objects contain an "Email" property the value will be passed through <see cref="EncryptionHelper.Decrypt"/>
        /// </summary>
        public async Task<List<T>> LoadAllAsync()
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            var json = await File.ReadAllTextAsync(_filePath);

            // Deserialize once and work with the result.
            var data = JsonSerializer.Deserialize<List<T>>(json);

            if (data == null)
                return new List<T>();

            // Decrypt any "Email" property found via reflection.
            // IMPORTANT:
            // - GetProperty is case-sensitive by default. This looks for a property named exactly "Email".
            // - Decrypt may throw if the stored value is not valid Base64 - consider validating/catching if needed.
            foreach (var item in data)
            {
                var type = item.GetType();

                var emailProp = type.GetProperty("Email");
                if (emailProp != null)
                {
                    var encrypted = emailProp.GetValue(item)?.ToString();
                    if (!string.IsNullOrEmpty(encrypted))
                        emailProp.SetValue(item, EncryptionHelper.Decrypt(encrypted));
                }
            }

            // Return the decrypted list. NOTE: original code re-deserialized here which would discard the decrypted changes.
            return data ?? new List<T>();
        }

        /// <summary>
        /// Save the provided list to disk as pretty-printed JSON.
        /// If an "Email" property exists it will be encrypted via <see cref="EncryptionHelper.Encrypt"/> before writing.
        /// </summary>
        /// <remarks>
        /// This method mutates the objects in-place (it sets the Email property to the encrypted value).
        /// If you want to keep in-memory objects unchanged, create a shallow copy of the list and/or objects before encrypting.
        /// </remarks>
        public async Task SaveAsync(List<T> data)
        {
            foreach (var item in data)
            {
                var type = item.GetType();

                // Encrypt Email if the field exists.
                var emailProp = type.GetProperty("Email");
                if (emailProp != null)
                {
                    var clean = emailProp.GetValue(item)?.ToString();
                    if (!string.IsNullOrEmpty(clean))
                        emailProp.SetValue(item, EncryptionHelper.Encrypt(clean));
                }
            }

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);

            // Consider: after writing you may want to decrypt the in-memory objects again (or operate on copies),
            // otherwise the passed `data` will remain containing encrypted emails.
        }

        /// <summary>
        /// Convenience: load list, append new item, save back to disk.
        /// </summary>
        public async Task AddAsync(T item)
        {
            var list = await LoadAllAsync();
            list.Add(item);
            await SaveAsync(list);
        }

    }
}
