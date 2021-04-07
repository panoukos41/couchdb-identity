# AspNetCore.Identity.CouchDB

[![Release](https://github.com/panoukos41/couchdb-identity/actions/workflows/release.yaml/badge.svg)](https://github.com/panoukos41/couchdb-identity/actions/workflows/release.yaml)
[![NuGet](https://buildstats.info/nuget/AspNetCore.Identity.CouchDB?includePreReleases=true)](https://www.nuget.org/packages/AspNetCore.Identity.CouchDB)
[![License](https://img.shields.io/github/license/panoukos41/couchdb-identity)](https://github.com/panoukos41/couchdb-identity/blob/main/LICENSE.md)

[CouchDB](https://couchdb.apache.org/) identity store provider for the [ASP.NET Core Identity](https://github.com/dotnet/aspnetcore/tree/main/src/Identity) using [CouchDB.NET](https://github.com/matteobortolazzo/couchdb-net).

The UserStore implements the following Interfaces:
- IQueryableUserStore
- IUserStore
- IUserPasswordStore
- IUserSecurityStampStore
- IUserEmailStore
- IUserPhoneNumberStore
- IUserRoleStore
- IUserClaimStore
- IUserLoginStore
- IUserTwoFactorStore
- IUserAuthenticatorKeyStore
- IUserLockoutStore

The RoleStore implements the following Interfaces:
- IQueryableRoleStore
- IRoleStore
- IRoleClaimStore

The project was insipired from the existing [AspNetCore.Identity.Cassandra](https://github.com/lkubis/AspNetCore.Identity.Cassandra) implementation.

# Before Getting started
This project was made in a hurry and was live tested on a really simple application so some things might not work as expected. Feel free to open an issue and even a pull request.

I will try my best to support the project and provide tests and samples whenever i find time.

Thanks for looking at this library 😄

# Getting Started

By default a database named `identity` is used but you can change it using the options overload.

Your [CouchDB](https://couchdb.apache.org/) database must implement the [ddoc.json](ddocs/ddoc.json) in the [ddocs](/ddocs) folder.

At your `Startup.cs` add your `ICouchClient` in DI and use the following code.
```csharp
using AspNetCore.Identity.CouchDB.Models;

// Configure Identity stores and services.
services.AddIdentity<CouchDbUser, CouchDbRole>()
        .AddCouchDbStores();
```

Or you can provide your own instance of `ICouchClient` in the options.
```csharp
// Configure Identity stores and services.
// With custom client.
services.AddIdentity<CouchDbUser, CouchDbRole>()
        .AddCouchDbStores(options => options
            .UseClient(new CouchClient("http://localhost:5984")));
```

You can also customize the `design document` and `views` used by the library. More information as to what a view returns and when can be found in the [ddocs](/ddocs) fodler.

```csharp
services.AddIdentity<CouchDbUser, CouchDbRole>()
        .AddCouchDbStores(options => options
            .UseViewOptions(new CouchDbViewOptions
            {
                Document = "identity";
                User = "user";
                UserNormalizedUsername = "user.normalized_username";
                UserNormalizedEmail = "user.normalized_email";
                UserRolesNormalizedName = "user.roles.normalized_name";
                UserClaims = "user.claims";
                UserLogins = "user.logins";
                Role = "role";
                RoleNormalizedName = "role.normalized_name";
            })));
```

Services are *registered as Singleton* since `ICouchClient` is/should be registered as such and no other dependency is needed.

# LICENSE
The project is Licensed under the MIT License see the [LICENSE](LICENSE.md) for more info.
