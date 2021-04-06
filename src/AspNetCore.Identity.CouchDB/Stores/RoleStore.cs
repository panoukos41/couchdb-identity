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
    public class RoleStore : RoleStore<CouchDbRole>
    {
        public RoleStore(IOptionsMonitor<CouchDbIdentityOptions> options, IServiceProvider provider)
            : base(options, provider)
        {
        }
    }

    /// <summary>
    /// Creates a new instance of a persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Nothing to dispose of.")]
    public class RoleStore<TRole> :
        StoreBase<TRole>,
        IQueryableRoleStore<TRole>,
        IRoleStore<TRole>,
        IRoleClaimStore<TRole>
        where TRole : CouchDbRole
    {
        public RoleStore(
            IOptionsMonitor<CouchDbIdentityOptions> options,
            IServiceProvider provider)
            : base(options, provider)
        {
            Discriminator = Options.CurrentValue.RoleDiscriminator;
        }

        /// <inheritdoc/>
        protected override string Discriminator { get; }

        /// <inheritdoc/>
        public void Dispose() { }

        #region IQueryableRoleStore

        /// <inheritdoc/>
        public virtual IQueryable<TRole> Roles => GetDatabase().AsQueryable();

        #endregion

        #region IRoleStore

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));

            role.Id ??= Guid.NewGuid().ToString();
            await GetDatabase().AddAsync(role, cancellationToken: cancellationToken).ConfigureAwait(false);

            return IdentityResult.Success;
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));

            await GetDatabase().AddOrUpdateAsync(role, cancellationToken: cancellationToken).ConfigureAwait(false);

            return IdentityResult.Success;
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));

            await GetDatabase().RemoveAsync(role, cancellationToken: cancellationToken).ConfigureAwait(false);

            return IdentityResult.Success;
        }

        /// <inheritdoc/>
        public virtual async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(roleId, nameof(roleId));
#nullable disable
            return await GetDatabase().FindAsync(roleId, cancellationToken: cancellationToken).ConfigureAwait(false);
#nullable enable
        }

        /// <inheritdoc/>
        public virtual async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(normalizedRoleName, nameof(normalizedRoleName));

            var options = new CouchViewOptions<string>
            {
                IncludeDocs = true,
                Key = normalizedRoleName
            };

#nullable disable
            return (await GetDatabase()
                .GetViewAsync(Views<CouchDbUser<TRole>, TRole>.RoleNormalizedName, options, cancellationToken)
                .ConfigureAwait(false))
                .FirstOrDefault()
                ?.Document;
#nullable enable
        }

        /// <inheritdoc/>
        public virtual Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.Id);
        }

        /// <inheritdoc/>
        public virtual Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.Name);
        }

        /// <inheritdoc/>
        public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        /// <inheritdoc/>
        public virtual Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));

            role.Name = roleName;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));

            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        #endregion

        #region IRoleClaimStore

        /// <inheritdoc/>
        public virtual Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));
            Check.NotNull(claim, nameof(claim));

            role.Claims.Add(claim);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));
            Check.NotNull(claim, nameof(claim));

            role.Claims.Remove(claim);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            Check.NotNull(role, nameof(role));

            return Task.FromResult((IList<Claim>)role.Claims.Select(x => x.ToClaim()).ToArray());
        }

        #endregion
    }
}