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

namespace Tests.Integration.Users
{
    public class UserViewTests : BaseTest<CouchDbUser, CouchDbRole>
    {
        // Here we try the user (count/all) view that is not used
        // inside the store implementations.

        private readonly ICouchDatabase<CouchDbUser> _db = MainCollection.GetDatabase<CouchDbUser>(MainCollection.Options.UserDiscriminator);

        private async Task ArrangeAsync()
        {
            var roles = new CouchDbUser[]
            {
                new() { UserName = "1" },
                new() { UserName = "2" }
            };

            await _db.AddOrUpdateRangeAsync(roles, default);
        }

        [Fact]
        public async Task Views_User_Should_Return()
        {
            // Arrange
            await ArrangeAsync();

            // Act
            var users = await GetViewAsync(Views<CouchDbUser, CouchDbRole>.User);

            // Assert
            users.Should().HaveCount(1);
            users[0].Key.Should().BeNull();
            int.Parse(users[0].Value).Should().BeGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task Views_User_WithDocs_Should_Return()
        {
            // Arrange
            await ArrangeAsync();

            // Act
            var roles = await GetViewAsync(Views<CouchDbUser, CouchDbRole>.User, new()
            {
                Reduce = false
            });

            // Assert
            roles.Should().HaveCountGreaterOrEqualTo(2);
        }

        // This is internal code.
        private Task<List<CouchView<TKey, TValue, CouchDbUser>>> GetViewAsync<TKey, TValue>(
            View<TKey, TValue, CouchDbUser> view,
            CouchViewOptions<TKey>? options = null,
            CancellationToken cancellationToken = default)
        {
            return _db.GetViewAsync<TKey, TValue>(view.Design, view.Value, options, cancellationToken);
        }
    }
}