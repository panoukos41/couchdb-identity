using CouchDB.Driver.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <summary>
    /// A CouchDb document representing a role.
    /// </summary>
    /// <remarks>
    /// ToString returns the <see cref="Name"/> property.<br/>
    /// Equality only compares the <see cref="NormalizedName"/> property.<br/>
    /// GetHashCode returns a hash for the <see cref="NormalizedName"/> property.
    /// </remarks>
    public class CouchDbRole : CouchDocument, IEquatable<CouchDbRole?>
    {
        /// <summary>
        /// Initialize a new <see cref="CouchDbRole"/>.
        /// </summary>
        public CouchDbRole()
        {
        }

        /// <summary>
        /// Initialize a new <see cref="CouchDbRole"/> with a role name.
        /// </summary>
        /// <param name="roleName">The role name to initialize with.</param>
        public CouchDbRole(string roleName)
        {
            Name = roleName;
        }

        /// <summary>
        /// Initializes a new <see cref="CouchDbRole"/> from another role.
        /// </summary>
        /// <param name="other">The role to copy values from.</param>
        public CouchDbRole(CouchDbRole other)
        {
            Id = other.Id;
            Rev = other.Rev;
            Name = other.Name;
            NormalizedName = other.NormalizedName;
        }

        ///<summary>
        /// Gets or sets the name for this role.
        ///</summary>
        [JsonProperty("name")]
        public virtual string Name { get; set; } = null!;

        ///<summary>
        /// Gets or sets the normalized name for this role.
        ///</summary>
        [JsonProperty("normalized_name")]
        public virtual string NormalizedName { get; set; } = null!;

        /// <summary>
        /// Gets the claims a role has.
        /// </summary>
        [JsonProperty("claims")]
        public virtual HashSet<ClaimInfo> Claims { get; private set; } = new();

        /// <summary>
        /// Returns the name of the role.
        /// </summary>
        /// <returns>The name of the role.</returns>
        public override string ToString() => Name;

        #region Equals and HashCode

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as CouchDbRole);
        }

        /// <inheritdoc/>
        public bool Equals(CouchDbRole? other)
        {
            return other is not null &&
                   NormalizedName == other.NormalizedName;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(NormalizedName);
        }

        public static bool operator ==(CouchDbRole? left, CouchDbRole? right)
        {
            return EqualityComparer<CouchDbRole>.Default.Equals(left!, right!);
        }

        public static bool operator !=(CouchDbRole? left, CouchDbRole? right)
        {
            return !(left == right);
        }

        #endregion
    }
}