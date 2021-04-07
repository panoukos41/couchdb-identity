using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AspNetCore.Identity.CouchDB.Models
{
    /// <summary>
    /// A class that represents login information in a user object.
    /// </summary>
    /// <remarks>
    /// This class is modeled after <see cref="UserLoginInfo"/>.<br/>
    /// ToString returns the <see cref="ProviderDisplayName"/> property.<br/>
    /// Equality only compares the <see cref="LoginProvider"/> and <see cref="ProviderKey"/> properties.<br/>
    /// GetHashCode returns a combined hash for <see cref="LoginProvider"/> and <see cref="ProviderKey"/> properties.<br/>
    /// Implicit operators to and from <see cref="UserLoginInfo"/> are provided.
    /// </remarks>
    [JsonObject("login")]
    public class LoginInfo : IEquatable<LoginInfo?>, IEquatable<UserLoginInfo?>
    {
        /// <summary>
        /// Creates a new instance of <see cref="LoginInfo"/>.
        /// </summary>
        /// <param name="loginProvider">The provider associated with this login information.</param>
        /// <param name="providerKey">The unique identifier for this user provided by the login provider.</param>
        /// <param name="displayName">The display name for this user provided by the login provider.</param>
        [JsonConstructor]
        public LoginInfo(string loginProvider, string providerKey, string displayName)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
            ProviderDisplayName = displayName;
        }

        /// <summary>
        /// Initializes a new <see cref="LoginInfo"/> from another.
        /// </summary>
        /// <param name="other">The <see cref="LoginInfo"/> to copy values from.</param>
        public LoginInfo(LoginInfo other) :
            this(other.LoginProvider, other.ProviderKey, other.ProviderDisplayName)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="UserLoginInfo"/> from another.
        /// </summary>
        /// <param name="other">The <see cref="UserLoginInfo"/> to copy values from.</param>
        public LoginInfo(UserLoginInfo other) :
            this(other.LoginProvider, other.ProviderKey, other.ProviderDisplayName)
        {
        }

        /// <summary>
        /// Gets or sets the provider for this instance of <see cref="LoginInfo"/>.
        /// </summary>
        /// <value>The provider for the this instance of Microsoft.AspNetCore.Identity.UserLoginInfo</value>
        /// <remarks>Examples of the provider may be Local, Facebook, Google, etc.</remarks>
        [JsonProperty("provider")]
        public string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user identity user provided by the login provider.
        /// </summary>
        /// <value>The unique identifier for the user identity user provided by the login provider.</value>
        /// <remarks>
        /// This would be unique per provider, examples may be @microsoft as a Twitter provider key.
        /// </remarks>
        [JsonProperty("key")]
        public string ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the display name for the provider.
        /// </summary>
        /// <value>The display name for the provider.</value>
        /// <remarks>Examples of the display name may be local, FACEBOOK, Google, etc.</remarks>
        [JsonProperty("display_name")]
        public string ProviderDisplayName { get; set; }

        /// <summary>
        /// Returns the <see cref="LoginProvider"/>.
        /// </summary>
        /// <returns>The <see cref="LoginProvider"/>.</returns>
        public override string ToString()
        {
            return ProviderDisplayName;
        }

        #region Implicit operators

        public static implicit operator LoginInfo(UserLoginInfo info) =>
            new(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName);

        public static implicit operator UserLoginInfo(LoginInfo info) =>
            new(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName);

        #endregion

        #region Equals and HashCode

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as LoginInfo);
        }

        /// <inheritdoc/>
        public bool Equals(LoginInfo? other)
        {
            return other is not null &&
                   LoginProvider == other.LoginProvider &&
                   ProviderKey == other.ProviderKey;
        }

        /// <inheritdoc/>
        public bool Equals(UserLoginInfo? other)
        {
            return other is not null &&
                LoginProvider == other.LoginProvider &&
                ProviderKey == other.ProviderKey;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(LoginProvider, ProviderKey);
        }

        public static bool operator ==(LoginInfo? left, LoginInfo? right)
        {
            return EqualityComparer<LoginInfo>.Default.Equals(left!, right!);
        }

        public static bool operator !=(LoginInfo? left, LoginInfo? right)
        {
            return !(left == right);
        }

        #endregion
    }
}