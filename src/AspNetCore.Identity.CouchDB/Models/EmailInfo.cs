using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <summary>
    /// A class that represents information about an email address in a user object.
    /// </summary>
    /// <remarks>
    /// ToString returns the <see cref="Address"/> property.<br/>
    /// Equality only compares the <see cref="Normalized"/> property.<br/>
    /// GetHashCode returns a hash for the <see cref="Normalized"/> property.
    /// </remarks>
    [JsonObject("email")]
    public class EmailInfo : IEquatable<EmailInfo?>
    {
        /// <summary>
        /// Initialize a new <see cref="EmailInfo"/> with the provided email.
        /// </summary>
        /// <param name="address">The email to initialize with.</param>
        [JsonConstructor]
        public EmailInfo(string address)
        {
            Address = address;
            Normalized = address.ToUpperInvariant();
        }

        /// <summary>
        /// Initializes a new <see cref="EmailInfo"/> from another.
        /// </summary>
        /// <param name="other">The <see cref="EmailInfo"/> to copy values from.</param>
        public EmailInfo(EmailInfo other)
        {
            Address = other.Address;
            Normalized = other.Normalized;
            Confirmed = other.Confirmed;
        }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [ProtectedPersonalData]
        [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Address { get; set; }

        /// <summary>
        /// Gets or sets the normalized email address.
        /// </summary>
        [JsonProperty("normalized", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Normalized { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating this email address has been confirmed.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        [PersonalData]
        [JsonProperty("confirmed", NullValueHandling = NullValueHandling.Ignore)]
        public virtual bool Confirmed { get; set; }

        /// <summary>
        /// Returns the email address.
        /// </summary>
        /// <returns>The email address.</returns>
        public override string ToString()
        {
            return Address;
        }

        #region Equals and HashCode

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as EmailInfo);
        }

        /// <inheritdoc/>
        public bool Equals(EmailInfo? other)
        {
            return other is not null &&
                   Normalized == other.Normalized;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Normalized);
        }

        public static bool operator ==(EmailInfo? left, EmailInfo? right)
        {
            return EqualityComparer<EmailInfo>.Default.Equals(left!, right!);
        }

        public static bool operator !=(EmailInfo? left, EmailInfo? right)
        {
            return !(left == right);
        }

        #endregion
    }
}