using AspNetCore.Identity.CouchDB.Models;
using FluentAssertions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration.Roles
{
    public class RoleClaimStoreTests : BaseTest<CouchDbUser, CouchDbRole>
    {
        private const string ClaimType = "role_claim_store";

        [Fact]
        public async Task AddAndRemoveRoleClaims()
        {
            // Arrange
            var role = await CreateRoleAsync();
            var c1 = CreateClaim(ClaimType);
            var c2 = CreateClaim(ClaimType);
            var claims = new[] { c1, c2 };

            // Act Add
            await RoleManager.AddClaimAsync(role, c1);
            await RoleManager.AddClaimAsync(role, c2);

            var find = await RoleManager.FindByIdAsync(role.Id);
            var findClaims = await RoleManager.GetClaimsAsync(find);

            // Assert Add
            Assert.Equal(claims, findClaims, ClaimComparer);

            // Act Remove
            await RoleManager.RemoveClaimAsync(role, c1);
            await RoleManager.RemoveClaimAsync(role, c2);

            find = await RoleManager.FindByIdAsync(role.Id);
            findClaims = await RoleManager.GetClaimsAsync(find);

            // Assert Remove
            findClaims.Should().BeEmpty();

            // Assert Null
            await Assert.ThrowsAsync<ArgumentNullException>(() => RoleManager.AddClaimAsync(null!, c1));
            await Assert.ThrowsAsync<ArgumentNullException>(() => RoleManager.AddClaimAsync(role, null!));

            await Assert.ThrowsAsync<ArgumentNullException>(() => RoleManager.RemoveClaimAsync(null!, c1));
            await Assert.ThrowsAsync<ArgumentNullException>(() => RoleManager.RemoveClaimAsync(role, null!));
        }
    }
}