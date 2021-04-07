using AspNetCore.Identity.CouchDB.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration.Users
{
    public class UserLoginStore : BaseTest<CouchDbUser, CouchDbRole>
    {
        private static UserLoginInfo CreateLogin()
        {
            var guid = Guid.NewGuid();
            return new(
                loginProvider: $"login_{guid}",
                providerKey: $"key_{guid}",
                displayName: $"display_{guid}");
        }

        [Fact]
        public async Task AddAndRemoveLogin()
        {
            // Arrange
            var user = await CreateUserAsync();
            var login = CreateLogin();

            // Act Add
            await UserManager.AddLoginAsync(user, login);
            var find = await FindUser(user.Id);
            var findlogins = await UserManager.GetLoginsAsync(user);

            // Assert Add
            findlogins.Should().Contain(x =>
                x.LoginProvider == login.LoginProvider &&
                x.ProviderKey == login.ProviderKey &&
                x.ProviderDisplayName == login.ProviderDisplayName);

            // Act Remove
            await UserManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            find = await FindUser(user.Id);
            findlogins = await UserManager.GetLoginsAsync(user);

            // Assert Remove
            findlogins.Should().BeEmpty();

            // Assert Null
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.GetLoginsAsync(null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.AddLoginAsync(null!, login));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.AddLoginAsync(user, null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.RemoveLoginAsync(null!, "", ""));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.RemoveLoginAsync(user, null!, ""));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.RemoveLoginAsync(user, "", null!));

            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.GetLoginsAsync(null!, default));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.AddLoginAsync(null!, login, default));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.AddLoginAsync(user, null!, default));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.RemoveLoginAsync(null!, "", "", default));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.RemoveLoginAsync(user, null!, "", default));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.RemoveLoginAsync(user, "", null!, default));
        }

        [Fact]
        public async Task FindByLoginAsync()
        {
            // Arrange
            var user = await CreateUserAsync();
            var login = CreateLogin();
            await UserManager.AddLoginAsync(user, login);

            // Act
            var find = await UserManager.FindByLoginAsync(login.LoginProvider, login.ProviderKey);

            // Assert
            find.Should().Be(user);
        }
    }
}