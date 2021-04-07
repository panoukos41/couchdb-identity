using AspNetCore.Identity.CouchDB.Internal;
using AspNetCore.Identity.CouchDB.Models;
using AspNetCore.Identity.CouchDB.Stores.Internal;
using CouchDB.Driver;
using CouchDB.Driver.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.CouchDB.Stores
{
    /// <inheritdoc/>
    public class UserStore : UserStore<CouchDbUser, CouchDbRole>
    {
        public UserStore(
            IOptionsMonitor<CouchDbIdentityOptions> options,
            IRoleStore<CouchDbRole> roleStore,
            IServiceProvider provider)
            : base(options, roleStore, provider)
        {
        }
    }

    /// <inheritdoc/>
    public class UserStore<TUser> : UserStore<TUser, CouchDbRole>
        where TUser : CouchDbUser
    {
        public UserStore(
            IOptionsMonitor<CouchDbIdentityOptions> options,
            IRoleStore<CouchDbRole> roleStore,
            IServiceProvider provider)
            : base(options, roleStore, provider)
        {
        }
    }

    /// <summary>
    /// Represents a new instance of a persistence store for the specified user type.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <remarks>This class has nothing to do with CouchDb's users database.</remarks>
    [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Nothing to dispose of.")]
    public class UserStore<TUser, TRole> :
        StoreBase<TUser>,
        IQueryableUserStore<TUser>,
        IUserStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserLoginStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserLockoutStore<TUser>
        where TUser : CouchDbUser<TRole>
        where TRole : CouchDbRole
    {
        public UserStore(
            IOptionsMonitor<CouchDbIdentityOptions> options,
            IRoleStore<TRole> roleStore,
            IServiceProvider provider)
            : base(options, provider)
        {
            _roleStore = roleStore;
            Discriminator = Options.CurrentValue.UserDiscriminator;
        }

        private readonly IRoleStore<TRole> _roleStore;

        /// <inheritdoc/>
        protected override string Discriminator { get; }

        /// <inheritdoc/>
        public virtual void Dispose() { }

        #region IQueryableUserStore

        /// <inheritdoc/>
        public virtual IQueryable<TUser> Users => GetDatabase().AsQueryable();

        #endregion

        #region IUserStore

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Id ??= Guid.NewGuid().ToString();
            await GetDatabase().AddAsync(user, cancellationToken: cancellationToken).ConfigureAwait(false);

            return IdentityResult.Success;
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            await GetDatabase().AddOrUpdateAsync(user, cancellationToken: cancellationToken).ConfigureAwait(false);

            return IdentityResult.Success;
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            await GetDatabase().RemoveAsync(user, cancellationToken: cancellationToken).ConfigureAwait(false);

            return IdentityResult.Success;
        }

        /// <inheritdoc/>
        public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(userId, nameof(userId));

#nullable disable
            return GetDatabase().FindAsync(userId, cancellationToken: cancellationToken);
#nullable enable
        }

        /// <inheritdoc/>
        public virtual async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(normalizedUserName, nameof(normalizedUserName));

            var options = new CouchViewOptions<string>
            {
                IncludeDocs = true,
                Key = normalizedUserName
            };

#nullable disable
            return (await GetDatabase()
                .GetViewAsync(Views<TUser, TRole>.UserNormalizedUsername, options, cancellationToken)
                .ConfigureAwait(false))
                .FirstOrDefault()
                ?.Document;
#nullable enable
        }

        /// <inheritdoc/>
        public virtual Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Id);
        }

        /// <inheritdoc/>
        public virtual Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.UserName);
        }

        /// <inheritdoc/>
        public virtual Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        /// <inheritdoc/>
        public virtual Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(userName, nameof(userName));

            user.UserName = userName;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(normalizedName, nameof(normalizedName));

            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        #endregion

        #region IUserPasswordStore

        /// <inheritdoc/>
        public virtual Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Password.Hash);
        }

        /// <inheritdoc/>
        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(passwordHash, nameof(passwordHash));

            user.Password.Hash = passwordHash;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(!string.IsNullOrEmpty(user.Password.Hash));
        }

        #endregion

        #region IUserSecurityStampStore

        /// <inheritdoc/>
        public virtual Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Password.Salt);
        }

        /// <inheritdoc/>
        public virtual Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(stamp, nameof(stamp));

            user.Password.Salt = stamp;
            return Task.CompletedTask;
        }

        #endregion

        #region IUserEmailStore

        /// <inheritdoc/>
        public virtual async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(normalizedEmail, nameof(normalizedEmail));

            var options = new CouchViewOptions<string>
            {
                IncludeDocs = true,
                Key = normalizedEmail
            };

#nullable disable
            return (await GetDatabase()
                .GetViewAsync(Views<TUser, TRole>.UserNormalizedEmail, options, cancellationToken)
                .ConfigureAwait(false))
                .FirstOrDefault()
                ?.Document;
#nullable enable
        }

        /// <inheritdoc/>
        public virtual Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Email.Address);
        }

        /// <inheritdoc/>
        public virtual Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Email.Normalized);
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Email.Confirmed);
        }

        /// <inheritdoc/>
        public virtual Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(email, nameof(email));

            user.Email.Address = email;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(normalizedEmail, nameof(normalizedEmail));

            user.Email.Normalized = normalizedEmail;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Email.Confirmed = confirmed;
            return Task.CompletedTask;
        }

        #endregion

        #region IUserPhoneNumberStore

        /// <inheritdoc/>
        public virtual Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Phone.Number);
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Phone.Confirmed);
        }

        /// <inheritdoc/>
        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(phoneNumber, nameof(phoneNumber));

            user.Phone.Number = phoneNumber;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Phone.Confirmed = confirmed;
            return Task.CompletedTask;
        }

        #endregion

        #region IUserRoleStore

        /// <inheritdoc/>
        public virtual async Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(normalizedRoleName, nameof(normalizedRoleName));

            var role = await _roleStore.FindByNameAsync(normalizedRoleName, cancellationToken);

            Check.Found(role, nameof(normalizedRoleName), $"The role '{normalizedRoleName}' does not exist.");

            user.Roles.Add(role);
        }

        /// <inheritdoc/>
        public virtual Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(normalizedRoleName, nameof(normalizedRoleName));

            user.Roles.RemoveWhere(x => x.NormalizedName == normalizedRoleName);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult((IList<string>)user.Roles.Select(x => x.Name).ToArray());
        }

        /// <inheritdoc/>
        public virtual async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(normalizedRoleName, nameof(normalizedRoleName));

            var options = new CouchViewOptions<string>
            {
                IncludeDocs = true,
                Key = normalizedRoleName
            };

            return (await GetDatabase()
                .GetViewAsync(Views<TUser, TRole>.UserRolesNormalizedName, options, cancellationToken)
                .ConfigureAwait(false))
                .Select(x => x.Document)
                .ToArray();
        }

        /// <inheritdoc/>
        public virtual Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(normalizedRoleName, nameof(normalizedRoleName));

            return Task.FromResult(user.Roles.Any(x => x.NormalizedName == normalizedRoleName));
        }

        #endregion

        #region IUserClaimStore

        /// <inheritdoc/>
        public virtual Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(claims, nameof(claims));

            foreach (var claim in claims)
                user.Claims.Add(claim);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(claims, nameof(claims));

            foreach (var claim in claims)
                user.Claims.Remove(claim);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(claim, nameof(claim));
            Check.NotNull(newClaim, nameof(newClaim));

            user.Claims.Remove(claim);
            user.Claims.Add(newClaim);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult((IList<Claim>)user.Claims.Select(x => x.ToClaim()).ToArray());
        }

        /// <inheritdoc/>
        public virtual async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(claim, nameof(claim));

            var options = new CouchViewOptions<string[]>
            {
                IncludeDocs = true,
                Key = new[] { claim.Type, claim.Value }
            };

            return (await GetDatabase()
                .GetViewAsync(Views<TUser, TRole>.UserClaims, options, cancellationToken)
                .ConfigureAwait(false))
                .Select(x => x.Document)
                .ToArray();
        }

        #endregion

        #region IUserLoginStore

        /// <inheritdoc/>
        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult((IList<UserLoginInfo>)user.Logins.Select(x => (UserLoginInfo)x).ToArray());
        }

        /// <inheritdoc/>
        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(login, nameof(login));

            user.Logins.Add(login);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            user.Logins.Remove(new(loginProvider, providerKey, ""));
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            var options = new CouchViewOptions<string[]>
            {
                IncludeDocs = true,
                Key = new[] { loginProvider, providerKey }
            };
#nullable disable
            return (await GetDatabase()
                .GetViewAsync(Views<TUser, TRole>.UserLogins, options, cancellationToken)
                .ConfigureAwait(false))
                .FirstOrDefault()
                ?.Document;
#nullable restore
        }

        #endregion

        #region IUserTwoFactorStore

        /// <inheritdoc/>
        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.TwoFactorEnabled);
        }

        /// <inheritdoc/>
        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        #endregion

        #region IUserAuthenticatorKeyStore

        /// <inheritdoc/>
        public virtual Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Authenticator.Key);
        }

        /// <inheritdoc/>
        public virtual Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(key, nameof(key));

            user.Authenticator.Key = key;
            return Task.CompletedTask;
        }

        #endregion

        #region IUserLockoutStore

        /// <inheritdoc/>
        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Lockout.End);
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Lockout.Enabled);
        }

        /// <inheritdoc/>
        public virtual Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Lockout.AccessFailedCount);
        }

        /// <inheritdoc/>
        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Lockout.End = lockoutEnd;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Lockout.Enabled = enabled;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Lockout.AccessFailedCount += 1;
            return Task.FromResult(user.Lockout.AccessFailedCount);
        }

        /// <inheritdoc/>
        public virtual Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Lockout.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        #endregion
    }
}