﻿using CouchDB.Driver.Types;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <inheritdoc/>
    public class CouchDbUser : CouchDbUser<CouchDbRole>
    {
        /// <inheritdoc/>
        public CouchDbUser()
        {
        }

        /// <inheritdoc/>
        public CouchDbUser(string userName) : base(userName)
        {
        }

        /// <inheritdoc/>
        public CouchDbUser(CouchDbUser other) : base(other)
        {
        }
    }

    /// <summary>
    /// A CouchDb document representing a user.
    /// </summary>
    /// <typeparam name="TRole">The type of roles the user stores.</typeparam>
    /// <remarks>
    /// This class has nothing to with CouchDb's users database.<br/>
    /// <br/>
    /// ToString returns the <see cref="UserName"/> property.<br/>
    /// Equality only compares the <see cref="NormalizedUserName"/> property.<br/>
    /// GetHashCode returns a hash for the <see cref="NormalizedUserName"/> property.
    /// </remarks>
    public class CouchDbUser<TRole> : CouchDocument, IEquatable<CouchDbUser<TRole>?>
        where TRole : CouchDbRole
    {
        /// <summary>
        /// Initialize a new <see cref="CouchDbUser"/>.
        /// </summary>
        public CouchDbUser() : this(string.Empty)
        {
        }

        /// <summary>
        /// Initialize a new <see cref="CouchDbUser"/> with a username.
        /// </summary>
        /// <param name="userName">The username to initialize with.</param>
        public CouchDbUser(string userName)
        {
            UserName = userName;
            NormalizedUserName = userName.ToUpperInvariant();
        }

        /// <summary>
        /// Initializes a new <see cref="CouchDbUser"/> from another.
        /// </summary>
        /// <param name="other">The user to copy values from.</param>
        public CouchDbUser(CouchDbUser<TRole> other)
        {
            Id = other.Id;
            Rev = other.Rev;
            UserName = other.UserName;
            NormalizedUserName = other.NormalizedUserName;
            Email = new(other.Email);
            Phone = new(other.Phone);
            Password = new(other.Password);
            Lockout = new(other.Lockout);
            TwoFactorEnabled = other.TwoFactorEnabled;
            Roles = new(other.Roles);
            Logins = new(other.Logins);
        }

        /// <inheritdoc/>
        [PersonalData]
        [DataMember]
        [JsonProperty("_id", NullValueHandling = NullValueHandling.Ignore)]
        public override string Id { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        [ProtectedPersonalData]
        [JsonProperty("username")]
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        [JsonProperty("normalized_username")]
        public virtual string NormalizedUserName { get; set; }

        /// <summary>
        /// A users full email info.
        /// </summary>
        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public virtual EmailInfo Email { get; set; } = new("");

        /// <summary>
        /// A users full phone info.
        /// </summary>
        [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
        public virtual PhoneInfo Phone { get; set; } = new("");

        /// <summary>
        /// A users password info.
        /// </summary>
        [JsonProperty("password")]
        public virtual PasswordInfo Password { get; set; } = new("", "");

        /// <summary>
        /// A users authenticator info.
        /// </summary>
        [JsonProperty("authenticator", NullValueHandling = NullValueHandling.Ignore)]
        public virtual AuthenticatorInfo Authenticator { get; set; } = new("");

        /// <summary>
        /// A users lockout info.
        /// </summary>
        [JsonProperty("lockout")]
        public virtual LockoutInfo Lockout { get; set; } = new();

        /// <summary>
        /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
        /// </summary>
        /// <value>True if 2fa is enabled, otherwise false.</value>
        [PersonalData]
        [JsonProperty("two_factor_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets the roles this user belongs to.
        /// </summary>
        [JsonProperty("roles")]
        public virtual HashSet<TRole> Roles { get; private set; } = new();
        
        /// <summary>
        /// Gets the claims a user has.
        /// </summary>
        [JsonProperty("claims")]
        public virtual HashSet<ClaimInfo> Claims { get; private set; } = new();

        /// <summary>
        /// Gets a list of a users login info.
        /// </summary>
        [JsonProperty("logins")]
        public virtual HashSet<LoginInfo> Logins { get; private set; } = new();

        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        /// <returns>The username of the user.</returns>
        public override string ToString() => UserName;

        #region Equals and HashCode

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as CouchDbUser<TRole>);
        }

        /// <inheritdoc/>
        public bool Equals(CouchDbUser<TRole>? other)
        {
            return other is not null &&
                Id == other.Id &&
                NormalizedUserName == other.NormalizedUserName;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, NormalizedUserName);
        }

        public static bool operator ==(CouchDbUser<TRole>? left, CouchDbUser<TRole>? right)
        {
            return EqualityComparer<CouchDbUser<TRole>>.Default.Equals(left!, right!);
        }

        public static bool operator !=(CouchDbUser<TRole>? left, CouchDbUser<TRole>? right)
        {
            return !(left == right);
        }

        #endregion
    }
}