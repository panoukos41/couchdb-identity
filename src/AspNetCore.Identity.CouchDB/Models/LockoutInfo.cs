using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <summary>
    /// A class that represents lockout information in a user object.
    /// </summary>
    [JsonObject("lockout")]
    public class LockoutInfo : IEquatable<LockoutInfo?>
    {
        /// <summary>
        /// Initialize a new <see cref="LockoutInfo"/>.
        /// </summary>
        public LockoutInfo()
        {
        }

        /// <summary>
        /// Initialize a new <see cref="LockoutInfo"/> with values.
        /// </summary>
        public LockoutInfo(int accessFailedCount = 0, bool enabled = false, DateTimeOffset? end = null)
        {
            AccessFailedCount = accessFailedCount;
            Enabled = enabled;
            End = end;
        }

        /// <summary>
        /// Initializes a new <see cref="LockoutInfo"/> from another.
        /// </summary>
        /// <param name="other">The <see cref="LockoutInfo"/> to copy values from.</param>
        public LockoutInfo(LockoutInfo other)
        {
            AccessFailedCount = other.AccessFailedCount;
            Enabled = other.Enabled;
            End = other.End;
        }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        [JsonProperty("access_failed_count", NullValueHandling = NullValueHandling.Ignore)]
        public virtual int AccessFailedCount { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        /// <value>True if the user could be locked out, otherwise false.</value>
        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>A value in the past means the user is not locked out.</remarks>
        [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTimeOffset? End { get; set; }

        #region Equals and HashCode

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as LockoutInfo);
        }

        /// <inheritdoc/>
        public bool Equals(LockoutInfo? other)
        {
            return other is not null &&
                   AccessFailedCount == other.AccessFailedCount &&
                   Enabled == other.Enabled &&
                   EqualityComparer<DateTimeOffset?>.Default.Equals(End, other.End);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(AccessFailedCount, Enabled, End);
        }

        public static bool operator ==(LockoutInfo? left, LockoutInfo? right)
        {
            return EqualityComparer<LockoutInfo>.Default.Equals(left!, right!);
        }

        public static bool operator !=(LockoutInfo? left, LockoutInfo? right)
        {
            return !(left == right);
        }

        #endregion
    }
}