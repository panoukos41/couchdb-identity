using AspNetCore.Identity.CouchDB.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration.Users
{
    public class UserStoreTests : BaseTest<CouchDbUser, CouchDbRole>
    {
        [Fact]
        public async Task Create()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var user = new CouchDbUser
            {
                Id = $"test_{guid}",
                UserName = $"ninja_{guid}",
                Email = new($"ninja_{guid}")
            };

            // Act
            var result = await UserManager.CreateAsync(user);

            // Assert
            result.Should().Be(IdentityResult.Success);
            user.NormalizedUserName.Should().Be(UserManager.KeyNormalizer.NormalizeName(user.NormalizedUserName));

            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.CreateAsync(null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.CreateAsync(null!, default));
        }

        [Fact]
        public async Task Update()
        {
            // Arrange
            var user = await CreateUserAsync();
            var update = new CouchDbUser(user)
            {
                UserName = $"{user.UserName}_{Guid.NewGuid()}"
            };

            // Act
            await UserManager.UpdateAsync(update);

            var find = await UserManager.FindByIdAsync(user.Id);

            // Assert
            find.Should().NotBeNull();
            find.Should().NotBeEquivalentTo(user);
            find.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task Delete()
        {
            // Arrange
            var user = await CreateUserAsync();

            // Act
            await UserManager.DeleteAsync(user);

            var find = await UserManager.FindByIdAsync(user.Id);

            // Assert
            find.Should().BeNull();

            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.DeleteAsync(null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.DeleteAsync(null!, default));
        }

        [Fact]
        public async Task Delete_Shallow()
        {
            // Arrange
            var user = await CreateUserAsync();

            // Act
            await UserManager.DeleteAsync(new() { Id = user.Id, Rev = user.Rev });

            var find = await UserManager.FindByIdAsync(user.Id);

            // Assert
            find.Should().BeNull();

            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.DeleteAsync(null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.DeleteAsync(null!, default));
        }

        [Fact]
        public async Task FindById()
        {
            // Arrange
            var user = await CreateUserAsync();

            // Act
            var find = await UserManager.FindByIdAsync(user.Id);

            // Assert
            find.Should().NotBeNull();
            find.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task FindByUserName()
        {
            // Arrange
            var user = await CreateUserAsync();

            // Act
            var find = await UserManager.FindByNameAsync(user.UserName);

            // Assert
            find.Should().NotBeNull();
            find.UserName.Should().Be(user.UserName);
        }
    }
}