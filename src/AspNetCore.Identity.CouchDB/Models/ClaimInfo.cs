using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <summary>
    /// A class tha trepresents <see cref="Claim"/> info on the database.
    /// </summary>
    public class ClaimInfo : IEquatable<ClaimInfo?>
    {
        /// <summary>
        /// Initialize a new <see cref="ClaimInfo"/> with a type and value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        [JsonConstructor]
        public ClaimInfo(string type, string value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Initializes by copying ClaimType and ClaimValue from the other claim.
        /// </summary>
        /// <param name="other">The claim to initialize from.</param>
        public ClaimInfo(Claim other) : this(other.Type, other.Value)
        {
        }

        /// <summary>
        /// Initializes by copying ClaimType and ClaimValue from the other claim.
        /// </summary>
        /// <param name="other">The claim to initialize from.</param>
        public ClaimInfo(ClaimInfo other) : this(other.Type, other.Value)
        {
        }

        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        [JsonProperty("type")]
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        [JsonProperty("value")]
        public virtual string Value { get; set; }

        /// <summary>
        /// Copies ClaimType and ClaimValue from the other claim.
        /// </summary>
        /// <param name="other">The claim to initialize from.</param>
        public virtual void FromClaim(Claim other)
        {
            Type = other.Type;
            Value = other.Value;
        }

        /// <summary>
        /// Copies ClaimType and ClaimValue from the other claim.
        /// </summary>
        /// <param name="other">The claim to initialize from.</param>
        public virtual void FromClaim(ClaimInfo other)
        {
            Type = other.Type;
            Value = other.Value;
        }

        /// <summary>
        /// Constructs a new claim with the type and value.
        /// </summary>
        /// <returns>The <see cref="Claim"/> that was produced.</returns>
        public virtual Claim ToClaim() => new(Type, Value);

        #region Implicit Operators

        public static implicit operator Claim(ClaimInfo claim) => claim.ToClaim();

        public static implicit operator ClaimInfo(Claim claim) => new(claim);

        #endregion

        #region Equals and HashCode

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as ClaimInfo);
        }

        /// <inheritdoc/>
        public bool Equals(ClaimInfo? other)
        {
            return other is not null &&
                   Type == other.Type &&
                   Value == other.Value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }

        public static bool operator ==(ClaimInfo left, ClaimInfo right)
        {
            return EqualityComparer<ClaimInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(ClaimInfo left, ClaimInfo right)
        {
            return !(left == right);
        }

        #endregion
    }
}