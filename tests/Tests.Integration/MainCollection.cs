using AspNetCore.Identity.CouchDB;
using AspNetCore.Identity.CouchDB.Models;
using AspNetCore.Identity.CouchDB.Stores;
using CouchDB.Driver;
using CouchDB.Driver.Types;
using Flurl.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration
{
    [CollectionDefinition(Name)]
    public class MainCollection : ICollectionFixture<MainCollection>, IAsyncLifetime
    {
        private static readonly ICouchClient _couchClient;
        private static readonly IServiceProvider _provider;

        public const string Name = "main_collection";

        public static CouchDbIdentityOptions Options => _provider.GetRequiredService<IOptions<CouchDbIdentityOptions>>().Value;

        static MainCollection()
        {
            _couchClient = new CouchClient(settings =>
            {
                settings.UseEndpoint("http://127.0.0.1:5984");
                settings.UseBasicAuthentication("admin", "admin");
            });

            IdentityBuilder builder = new(typeof(CouchDbUser), typeof(CouchDbRole), new ServiceCollection());

            builder.Services.AddIdentity<CouchDbUser, CouchDbRole>(config =>
            {
                config.Lockout = new LockoutOptions() { MaxFailedAccessAttempts = 2 };
                config.Password.RequireDigit = false;
                config.Password.RequiredLength = 3;
                config.Password.RequireLowercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            });
            builder.AddCouchDbStores(config =>
            {
                config.SetDatabaseName("test_identity");
                config.UseClient(_couchClient);
            });

            builder.Services.AddLogging();

            _provider = builder.Services.BuildServiceProvider();
        }

        public static UserManager<TUser> GetUserManager<TUser, TRole>()
            where TUser : CouchDbUser<TRole>
            where TRole : CouchDbRole
        {
            return _provider.GetRequiredService<UserManager<TUser>>();
        }

        public static RoleManager<TRole> GetRoleManager<TRole>()
            where TRole : CouchDbRole
        {
            return _provider.GetRequiredService<RoleManager<TRole>>();
        }

        public static UserStore<TUser, TRole> GetUserStore<TUser, TRole>()
            where TUser : CouchDbUser<TRole>
            where TRole : CouchDbRole
        {
            return (UserStore<TUser, TRole>)_provider.GetRequiredService<IUserStore<TUser>>();
        }

        public static RoleStore<TRole> GetRoleStore<TRole>()
            where TRole : CouchDbRole
        {
            return (RoleStore<TRole>)_provider.GetRequiredService<IRoleStore<TRole>>();
        }

        public static ICouchDatabase<T> GetDatabase<T>(string discriminator) where T : CouchDocument
        {
            return _couchClient.GetDatabase<T>(Options.DatabaseName, discriminator);
        }

        public async Task InitializeAsync()
        {
            var db = await Options.CouchClient!.GetOrCreateDatabaseAsync<Doc>(Options.DatabaseName);

            var ddoc = await File.ReadAllTextAsync("ddoc.json");
            var content = new StringContent(ddoc, Encoding.UTF8, "application/json");

            if (await db.FindAsync("_design/identity") is { } doc)
            {
                await db.RemoveAsync(doc);
            }

            await db.NewRequest().PostAsync(content);
        }

        public async Task DisposeAsync()
        {
            await Options.CouchClient!.DeleteDatabaseAsync(Options.DatabaseName);
        }

        private class Doc : CouchDocument { }
    }
}