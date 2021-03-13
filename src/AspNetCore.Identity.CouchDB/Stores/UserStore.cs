using AspNetCore.Identity.CouchDB.Internal;
using AspNetCore.Identity.CouchDB.Models;
using AspNetCore.Identity.CouchDB.Stores.Internal;
using CouchDB.Driver;
using CouchDB.Driver.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.CouchDB.Stores
{
    /// <inheritdoc/>
    public class UserStore : UserStore<CouchDbUser, CouchDbRole>
    {
        public UserStore(
            IServiceProvider provider,
            IOptionsMonitor<CouchDbIdentityOptions> options)
            : base(provider, options)
        {
        }
    }

    /// <inheritdoc/>
    public class UserStore<TUser> : UserStore<TUser, CouchDbRole>
        where TUser : CouchDbUser
    {
        public UserStore(
            IServiceProvider provider,
            IOptionsMonitor<CouchDbIdentityOptions> options)
            : base(provider, options)
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
        //IUserClaimStore<TUser>,
        IUserLoginStore<TUser>,
        IUserTwoFactorStore<TUser>,
        //IUserAuthenticatorKeyStore<TUser>,
        IUserLockoutStore<TUser>
        where TUser : CouchDbUser<TRole>
        where TRole : CouchDbRole
    {
        public UserStore(
            IServiceProvider provider,
            IOptionsMonitor<CouchDbIdentityOptions> options)
            : base(provider, options)
        {
            roleStore = provider.GetRequiredService<IRoleStore<TRole>>();
            Discriminator = Options.CurrentValue.UserDiscriminator;
        }

        private readonly IRoleStore<TRole> roleStore;

        /// <inheritdoc/>
        protected override string Discriminator { get; }

        /// <inheritdoc/>
        public void Dispose() { }

        #region IQueryableUserStore

        public IQueryable<TUser> Users => GetDatabase().AsQueryable();

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
                .GetViewAsync(Views.User<TUser, TRole>.NormalizedUserName, options, cancellationToken)
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
                .GetViewAsync(Views.User<TUser, TRole>.NormalizedEmail, options, cancellationToken)
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

            var role = await roleStore.FindByNameAsync(normalizedRoleName, cancellationToken);

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

            return Task.FromResult((IList<string>)user.Roles.ToList());
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
                .GetViewAsync(Views.User<TUser, TRole>.NormalizedRoleNames, options, cancellationToken)
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

        #region IUserLoginStore

        /// <inheritdoc/>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult((IList<UserLoginInfo>)user.Logins.Select(x => (UserLoginInfo)x).ToArray());
        }

        /// <inheritdoc/>
        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Logins.Add(login);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Logins.Remove(new(loginProvider, providerKey, ""));
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            // todo: Implement View
            throw new NotImplementedException();
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

        #region IUserLockoutStore

        /// <inheritdoc/>
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Lockout.End);
        }

        /// <inheritdoc/>
        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Lockout.Enabled);
        }

        /// <inheritdoc/>
        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Lockout.AccessFailedCount);
        }

        /// <inheritdoc/>
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Lockout.End = lockoutEnd;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Lockout.Enabled = enabled;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Lockout.AccessFailedCount += 1;
            return Task.FromResult(user.Lockout.AccessFailedCount);
        }

        /// <inheritdoc/>
        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            user.Lockout.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        #endregion
    }
}