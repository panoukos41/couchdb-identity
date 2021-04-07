using AspNetCore.Identity.CouchDB.Models;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration.Users
{
    public class UserEmailStore : BaseTest<CouchDbUser, CouchDbRole>
    {
        [Fact]
        public async Task FindByEmail()
        {
            // Arrange
            var user = await CreateUserAsync();
            await UserManager.SetEmailAsync(user, $"test_{Guid.NewGuid()}@test.com");

            // Act
            var find = await UserManager.FindByEmailAsync(user.Email.Address);

            // Assert
            find.Should().Be(user);

            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.FindByEmailAsync(null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.FindByEmailAsync(null!, default));
        }
    }
}