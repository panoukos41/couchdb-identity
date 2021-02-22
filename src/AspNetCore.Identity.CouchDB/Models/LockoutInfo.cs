using Newtonsoft.Json;
using System;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <summary>
    /// A class that represents lockout information in a user object.
    /// </summary>
    [JsonObject("lockout")]
    public class LockoutInfo
    {
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
    }
}