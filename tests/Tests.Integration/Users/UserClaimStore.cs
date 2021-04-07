using AspNetCore.Identity.CouchDB.Models;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration.Users
{
    public class UserClaimStore : BaseTest<CouchDbUser, CouchDbRole>
    {
        private const string ClaimType = "user_claim_store";

        [Fact]
        public async Task AddAndRemoveUserClaims()
        {
            // Arrange
            var user = await CreateUserAsync();
            var c1 = CreateClaim(ClaimType);
            var c2 = CreateClaim(ClaimType);
            var claims = new[] { c1, c2 };

            // Act Add
            await UserManager.AddClaimsAsync(user, claims);
            var find = await UserManager.FindByIdAsync(user.Id);
            var findClaims = await UserManager.GetClaimsAsync(find);

            // Assert Add
            Assert.Equal(claims, findClaims, ClaimComparer);

            // Act Remove
            await UserManager.RemoveClaimsAsync(user, claims);

            find = await UserManager.FindByIdAsync(user.Id);
            findClaims = await UserManager.GetClaimsAsync(find);

            // Assert Remove
            findClaims.Should().BeEmpty();

            // Assert Null
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.AddClaimAsync(null!, c1));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.AddClaimAsync(user, null!));

            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.RemoveClaimAsync(null!, c1));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.RemoveClaimAsync(user, null!));
        }

        [Fact]
        public async Task ReplaceUserClaims()
        {
            // Arrange
            var user = await CreateUserAsync();
            var claim = CreateClaim(ClaimType);
            var newClaim = CreateClaim(ClaimType);
            await UserManager.AddClaimAsync(user, claim);

            // Act
            await UserManager.ReplaceClaimAsync(user, claim, newClaim);
            var find = await UserManager.FindByIdAsync(user.Id);
            var findClaims = await UserManager.GetClaimsAsync(find);

            // Assert Add
            findClaims.Should()
                .Contain(x => ClaimEquals(x, newClaim))
                .And.NotContain(x => ClaimEquals(x, claim));
        }

        [Fact]
        public async Task GetUsersForClaim()
        {
            // Arrange
            var claim = CreateClaim(ClaimType);
            var user1 = await CreateUserAsync();
            var user2 = await CreateUserAsync();
            var users = new[] { user1, user2 };

            await UserManager.AddClaimAsync(user1, claim);
            await UserManager.AddClaimAsync(user2, claim);

            // Act
            var find = await UserManager.GetUsersForClaimAsync(claim);

            // Assert
            find.Should().Contain(users);
        }
    }
}