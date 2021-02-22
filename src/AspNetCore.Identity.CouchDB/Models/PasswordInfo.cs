using Newtonsoft.Json;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <summary>
    /// A class that represents password information in a user object.
    /// </summary>
    [JsonObject("password")]
    public class PasswordInfo
    {
        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Hash { get; set; } = null!;

        /// <summary>
        /// A random value that must change whenever a users credentials change (password
        /// changed, login removed)
        /// </summary>
        [JsonProperty("salt", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Salt { get; set; } = null!;
    }
}