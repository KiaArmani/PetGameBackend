using System;
using System.IO;
using static System.String;

namespace PetGameBackend.Validators
{
    public static class DataValidator
    {
        /// <summary>
        ///     Checks if the Key-Value-Pair consists of valid values
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void ValidateField(string key, string value)
        {
            CheckKey(key);

            if (IsNullOrEmpty(value))
                throw new InvalidDataException($"DataValidator (ValidateField) - Payload does not contain {key}.");
        }

        /// <summary>
        ///     Checks if the Key-Value-Pair consists of valid values
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void ValidateField(string key, int value)
        {
            CheckKey(key);

            if (value < 0)
                throw new InvalidDataException(
                    $"DataValidator (ValidateField) - Payload contains invalid value for {key} ({value}).");
        }

        /// <summary>
        ///     Checks if the provided <see cref="Enum" /> contains the provided value
        /// </summary>
        /// <param name="type">Enum to lookup</param>
        /// <param name="value">Value to lookup</param>
        public static void ValidateEnumField(Type type, object value)
        {
            if (!Enum.IsDefined(type, value))
                throw new InvalidDataException(
                    $"DataValidator (ValidateEnumField) - The provided value couldn't be found in the provided enum. ({type}, {value})");
        }

        /// <summary>
        ///     Checks if the provided string is a valid <see cref="Guid" />
        /// </summary>
        /// <param name="identifier"></param>
        public static void ValidateGuid(string identifier)
        {
            if (!Guid.TryParse(identifier, out _))
                throw new InvalidCastException(
                    $"DataValidator (ValidateGuid) - The provided identifier couldn't be parsed as a Guid. ({identifier})");
        }

        /// <summary>
        ///     Checks if the provided string <see cref="string.IsNullOrEmpty" /> and throws an <see cref="InvalidDataException" />
        /// </summary>
        /// <param name="key"></param>
        private static void CheckKey(string key)
        {
            if (IsNullOrEmpty(key))
                throw new InvalidDataException("DataValidator (ValidateField) - No Key supplied.");
        }
    }
}