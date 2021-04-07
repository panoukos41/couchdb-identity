using AspNetCore.Identity.CouchDB.Models;
using AspNetCore.Identity.CouchDB.Stores;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration
{
    [Collection(MainCollection.Name)]
    public abstract class BaseTest<TUser, TRole>
        where TUser : CouchDbUser<TRole>, new()
        where TRole : CouchDbRole, new()
    {
        protected static UserStore<TUser, TRole> UserStore => MainCollection.GetUserStore<TUser, TRole>();

        protected static UserManager<TUser> UserManager => MainCollection.GetUserManager<TUser, TRole>();

        protected static RoleStore<TRole> RoleStore => MainCollection.GetRoleStore<TRole>();

        protected static RoleManager<TRole> RoleManager => MainCollection.GetRoleManager<TRole>();

        protected static ClaimEquality ClaimComparer { get; } = new();

        protected static async Task<TUser> CreateUserAsync()
        {
            var guid = Guid.NewGuid();
            var user = new TUser
            {
                Id = $"test_{guid}",
                UserName = $"user_{guid}",
                Email = new($"email_{guid}")
            };

            await UserManager.CreateAsync(user);
            return user;
        }

        protected static async Task<TRole> CreateRoleAsync()
        {
            var guid = Guid.NewGuid();
            var role = new TRole
            {
                Id = $"test_{guid}",
                Name = $"role_{guid}"
            };

            await RoleManager.CreateAsync(role);
            return role;
        }

        protected static Claim CreateClaim(string claimType)
        {
            return new Claim(claimType, $"{Guid.NewGuid()}");
        }

        protected static bool ClaimEquals(Claim x, Claim y) => ClaimComparer.Equals(x, y);

        protected static async Task<TUser> FindUser(string id)
        {
            return await UserManager.FindByIdAsync(id);
        }

        protected static async Task<TRole> FindRole(string id)
        {
            return await RoleManager.FindByIdAsync(id);
        }

        protected class ClaimEquality : IEqualityComparer<Claim>
        {
            public bool Equals(Claim? x, Claim? y) =>
                x is not null && y is not null &&
                x.Type == y.Type && x.Value == y.Value;

            public int GetHashCode([DisallowNull] Claim obj) =>
                HashCode.Combine(obj.Type, obj.Value);
        }
    }
}