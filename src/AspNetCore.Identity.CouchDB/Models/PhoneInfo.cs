using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <summary>
    /// A class that represents phone information in a user object.
    /// </summary>
    /// <remarks>
    /// ToString returns the <see cref="Number"/> property.<br/>
    /// Equality only compares the <see cref="Number"/> property.<br/>
    /// GetHashCode returns a hash for the <see cref="Number"/> property.<br/>
    /// </remarks>
    [JsonObject("phone")]
    public class PhoneInfo : IEquatable<PhoneInfo?>
    {
        /// <summary>
        /// Gets or sets a telephone number for the user.
        /// </summary>
        [ProtectedPersonalData]
        [JsonProperty("number", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Number { get; set; } = null!;

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their telephone address.
        /// </summary>
        /// <value>True if the telephone number has been confirmed, otherwise false.</value>
        [PersonalData]
        [JsonProperty("confirmed", NullValueHandling = NullValueHandling.Ignore)]
        public virtual bool Confirmed { get; set; }

        /// <summary>
        /// Returns the <see cref="Number"/>.
        /// </summary>
        /// <returns>The <see cref="Number"/>.</returns>
        public override string ToString()
        {
            return Number;
        }

        #region Equals

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as PhoneInfo);
        }

        /// <inheritdoc/>
        public bool Equals(PhoneInfo? other)
        {
            return other is not null &&
                   Number == other.Number;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Number);
        }

        public static bool operator ==(PhoneInfo? left, PhoneInfo? right)
        {
            return EqualityComparer<PhoneInfo>.Default.Equals(left!, right!);
        }

        public static bool operator !=(PhoneInfo? left, PhoneInfo? right)
        {
            return !(left == right);
        }

        #endregion
    }
}