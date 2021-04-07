using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <summary>
    /// A class that represents password information in a user object.
    /// </summary>
    [JsonObject("password")]
    public class PasswordInfo : IEquatable<PasswordInfo?>
    {
        /// <summary>
        /// Initialize a new <see cref="PasswordInfo"/>.
        /// </summary>
        [JsonConstructor]
        public PasswordInfo(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }

        /// <summary>
        /// Initializes a new <see cref="PasswordInfo"/> from another.
        /// </summary>
        /// <param name="other">The <see cref="PasswordInfo"/> to copy values from.</param>
        public PasswordInfo(PasswordInfo other)
        {
            Hash = other.Hash;
            Salt = other.Salt;
        }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Hash { get; set; }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password
        /// changed, login removed)
        /// </summary>
        [JsonProperty("salt", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Salt { get; set; }

        #region  Equals and HashCode

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as PasswordInfo);
        }

        /// <inheritdoc/>
        public bool Equals(PasswordInfo? other)
        {
            return other is not null &&
                   Hash == other.Hash &&
                   Salt == other.Salt;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Hash, Salt);
        }

        public static bool operator ==(PasswordInfo? left, PasswordInfo? right)
        {
            return EqualityComparer<PasswordInfo>.Default.Equals(left!, right!);
        }

        public static bool operator !=(PasswordInfo? left, PasswordInfo? right)
        {
            return !(left == right);
        }

        #endregion
    }
}