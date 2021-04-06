using AspNetCore.Identity.CouchDB.Internal;
using AspNetCore.Identity.CouchDB.Models;
using CouchDB.Driver;
using CouchDB.Driver.Views;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration.Roles
{
    public class RoleViewTests : BaseTest<CouchDbUser, CouchDbRole>
    {
        // Here we try the role (count/all) view that is not used
        // inside the store implementations.

        private readonly ICouchDatabase<CouchDbRole> _db = MainCollection.GetDatabase<CouchDbRole>(MainCollection.Options.RoleDiscriminator);

        private async Task ArrangeAsync()
        {
            var guid = Guid.NewGuid();
            var roles = new CouchDbRole[]
            {
                new() { Name = $"{guid}_1" },
                new() { Name = $"{guid}_2" }
            };

            await _db.AddOrUpdateRangeAsync(roles, default);
        }

        [Fact]
        public async Task Views_Role_Should_Return()
        {
            // Arrange
            await ArrangeAsync();

            // Act
            var roles = await GetViewAsync(Views<CouchDbUser, CouchDbRole>.Role);

            // Assert
            roles.Should().HaveCount(1);
            roles[0].Key.Should().BeNull();
            int.Parse(roles[0].Value).Should().BeGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task Views_Role_WithDocs_Should_Return()
        {
            // Arrange
            await ArrangeAsync();

            // Act
            var roles = await GetViewAsync(Views<CouchDbUser, CouchDbRole>.Role, new()
            {
                Reduce = false
            });

            // Assert
            roles.Should().HaveCountGreaterOrEqualTo(2);
        }

        // This is internal code.
        private Task<List<CouchView<TKey, TValue, CouchDbRole>>> GetViewAsync<TKey, TValue>(
            View<TKey, TValue, CouchDbRole> view,
            CouchViewOptions<TKey>? options = null,
            CancellationToken cancellationToken = default)
        {
            return _db.GetViewAsync<TKey, TValue>(view.Design, view.Value, options, cancellationToken);
        }
    }
}