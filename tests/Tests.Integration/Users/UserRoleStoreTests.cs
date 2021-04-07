using AspNetCore.Identity.CouchDB.Models;
using AspNetCore.Identity.CouchDB.Stores;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration.Users
{
    public class UserRoleStoreTests : BaseTest<CouchDbUser, CouchDbRole>
    {
        [Fact]
        public async Task AddToAndRemoveFromRole()
        {
            // Arrange
            var user = await CreateUserAsync();
            var role = await CreateRoleAsync();

            // Act Add
            await UserManager.AddToRoleAsync(user, role.Name);
            var find = await FindUser(user.Id);
            var findRoles = await UserManager.GetRolesAsync(find);

            // Assert Add
            findRoles.Should().Contain(role.Name);

            // Act Remove
            await UserManager.RemoveFromRoleAsync(user, role.Name);
            find = await FindUser(user.Id);
            findRoles = await UserManager.GetRolesAsync(find);

            // Assert Remove
            findRoles.Should().BeEmpty();

            // Assert Nulls
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.AddToRoleAsync(user, null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.AddToRoleAsync(user, null!, default));

            await Assert.ThrowsAsync<ArgumentNullException>(() => UserManager.AddToRoleAsync(null!, "role"));
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.AddToRoleAsync(null!, "role", default));
        }

        [Fact]
        public async Task AddToAndRemoveFromRoles()
        {
            // Arrange
            var user = await CreateUserAsync();
            var role1 = await CreateRoleAsync();
            var role2 = await CreateRoleAsync();
            var roles = new[] { role1.Name, role2.Name };

            // Act Add
            await UserManager.AddToRolesAsync(user, roles);
            var find = await FindUser(user.Id);
            var findRoles = await UserManager.GetRolesAsync(find);

            // Assert Add
            findRoles.Should().Contain(roles);

            // Act Remove
            await UserManager.RemoveFromRolesAsync(user, new[] { role1.Name, role2.Name });
            find = await FindUser(user.Id);
            findRoles = await UserManager.GetRolesAsync(find);

            // Assert Remove
            findRoles.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRoles()
        {
            // Arrange
            var user = await CreateUserAsync();
            var role1 = await CreateRoleAsync();
            var role2 = await CreateRoleAsync();
            var roles = new[] { role1.Name, role2.Name };

            // Act
            await UserManager.AddToRolesAsync(user, roles);
            var find = await FindUser(user.Id);
            var findRoles = await UserManager.GetRolesAsync(find);

            // Assert
            findRoles.Should().Contain(roles);
        }

        [Fact]
        public async Task GetUsersInRole()
        {
            // Arrange
            var role = await CreateRoleAsync();
            var user1 = await CreateUserAsync();
            var user2 = await CreateUserAsync();
            var users = new[] { user1, user2 };

            await UserManager.AddToRoleAsync(user1, role.Name);
            await UserManager.AddToRoleAsync(user2, role.Name);

            // Act
            var findUsers = await UserManager.GetUsersInRoleAsync(role.Name);

            // Assert
            findUsers.Should().Contain(users);
        }

        [Fact]
        public async Task IsInRoleAsync()
        {
            // Arrange
            var role = await CreateRoleAsync();
            var user = await CreateUserAsync();

            await UserManager.AddToRoleAsync(user, role.Name);

            // Act
            var find = await FindUser(user.Id);
            var isInRole = await UserManager.IsInRoleAsync(find, role.Name);
            var isNotInRole = await UserManager.IsInRoleAsync(find, "not_a_Role");

            // Assert
            isInRole.Should().BeTrue();
            isNotInRole.Should().BeFalse();
        }
    }
}