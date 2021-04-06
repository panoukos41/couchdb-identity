using AspNetCore.Identity.CouchDB.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration.Roles
{
    public class RoleStoreTests : BaseTest<CouchDbUser, CouchDbRole>
    {
        [Fact]
        public async Task Create()
        {
            // Arrange
            var role = new CouchDbRole
            {
                Id = $"test_{Guid.NewGuid()}",
                Name = "ninja"
            };

            // Act
            var result = await RoleManager.CreateAsync(role);

            // Assert
            result.Should().Be(IdentityResult.Success);
            role.NormalizedName.Should().Be(RoleManager.KeyNormalizer.NormalizeName(role.Name));

            await Assert.ThrowsAsync<ArgumentNullException>(() => RoleManager.CreateAsync(null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => RoleStore.CreateAsync(null!, default));
        }

        [Fact]
        public async Task Update()
        {
            // Arrange
            var role = await CreateRoleAsync();
            var update = new CouchDbRole(role)
            {
                Name = $"{role.Name}_{Guid.NewGuid()}"
            };

            // Act
            await RoleManager.UpdateAsync(update);

            var find = await RoleManager.FindByIdAsync(role.Id);

            // Assert
            find.Should().NotBeNull();
            find.Should().NotBeEquivalentTo(role);
            find.Id.Should().Be(role.Id);
        }

        [Fact]
        public async Task Delete()
        {
            // Arrange
            var role = await CreateRoleAsync();

            // Act
            await RoleManager.DeleteAsync(role);

            var find = await RoleManager.FindByIdAsync(role.Id);

            // Assert
            find.Should().BeNull();

            await Assert.ThrowsAsync<ArgumentNullException>(() => RoleManager.DeleteAsync(null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => RoleStore.DeleteAsync(null!, default));
        }

        [Fact]
        public async Task Delete_Shallow()
        {
            // Arrange
            var role = await CreateRoleAsync();

            // Act
            await RoleManager.DeleteAsync(new() { Id = role.Id, Rev = role.Rev });

            var find = await RoleManager.FindByIdAsync(role.Id);

            // Assert
            find.Should().BeNull();

            await Assert.ThrowsAsync<ArgumentNullException>(() => RoleManager.DeleteAsync(null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => RoleStore.DeleteAsync(null!, default));
        }

        [Fact]
        public async Task FindById()
        {
            // Arrange
            var role = await CreateRoleAsync();

            // Act
            var find = await RoleManager.FindByIdAsync(role.Id);

            // Assert
            find.Should().NotBeNull();
            find.Id.Should().Be(role.Id);
        }

        [Fact]
        public async Task FindByRoleName()
        {
            // Arrange
            var role = await CreateRoleAsync();

            // Act
            var find = await RoleManager.FindByNameAsync(role.Name);

            // Assert
            find.Should().NotBeNull();
            find.Name.Should().Be(role.Name);
        }
    }
}