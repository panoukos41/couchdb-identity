using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <summary>
    /// A class that represents authenticator information in a user object.
    /// </summary>
    [JsonObject("authenticator")]
    public class AuthenticatorInfo : IEquatable<AuthenticatorInfo?>
    {
        /// <summary>
        /// Initialize a new instance of <see cref="AuthenticatorInfo"/> with a key.
        /// </summary>
        [JsonConstructor]
        public AuthenticatorInfo(string key)
        {
            Key = key;
        }

        /// <summary>
        /// The authenticator key.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        #region Implicit Operators

        public static implicit operator string?(AuthenticatorInfo info) => info.Key;

        public static implicit operator AuthenticatorInfo?(string key) => new(key);

        #endregion

        #region Equals and HashCode

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as AuthenticatorInfo);
        }

        /// <inheritdoc/>
        public bool Equals(AuthenticatorInfo? other)
        {
            return other is not null &&
                   Key == other.Key;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }

        public static bool operator ==(AuthenticatorInfo? left, AuthenticatorInfo? right)
        {
            return EqualityComparer<AuthenticatorInfo>.Default.Equals(left!, right!);
        }

        public static bool operator !=(AuthenticatorInfo? left, AuthenticatorInfo? right)
        {
            return !(left == right);
        }

        #endregion
    }
}