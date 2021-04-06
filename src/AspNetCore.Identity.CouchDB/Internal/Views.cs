using AspNetCore.Identity.CouchDB.Models;

namespace AspNetCore.Identity.CouchDB.Internal
{
    /// <summary>
    /// Views regarding AspNetCore.Identity.CouchDb integration.
    /// </summary>
    /// <remarks>
    /// These can be overriden but make sure your database has the new views
    /// and that they perform the correct actions.
    /// </remarks>
    public static class Views<TUser, TRole>
        where TUser : CouchDbUser<TRole>
        where TRole : CouchDbRole
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Since ApplyOptions is called with default options none of the values will be null.
        static Views() => ApplyOptions(new());

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static string Document { get; set; }

        /// <summary>
        /// With reduce = true (default) then Key = null, Value = 'count'<br/>
        /// When reduce = false (manual) then Key = Id, Value = Rev
        /// </summary>
        /// <remarks>Since couchdb.net uses flurl and that uses JSON.NET the int value json will be converted to string.</remarks>
        public static View<string, string, TUser> User { get; set; }

        /// <summary>
        /// Key = NormalizedUserName, Value = Rev
        /// </summary>
        public static View<string, string, TUser> UserNormalizedUsername { get; set; }

        /// <summary>
        /// Key = NormalizedEmail, Value = Rev
        /// </summary>
        public static View<string, string, TUser> UserNormalizedEmail { get; set; }

        /// <summary>
        /// Key = Role.NormalizedName, Value = Rev
        /// </summary>
        public static View<string, string, TUser> UserRolesNormalizedName { get; set; }

        /// <summary>
        /// Key = [Type, Value], Value = Rev
        /// </summary>
        public static View<string[], string, TUser> UserClaims { get; set; }

        /// <summary>
        /// Key = [LoginProvider, ProviderKey], Value = Rev
        /// </summary>
        public static View<string[], string, TUser> UserLogins { get; set; }

        /// <summary>
        /// With reduce = true (default) then Key = null, Value = 'count'<br/>
        /// When reduce = false (manual) then Key = Id, Value = Rev
        /// </summary>
        /// <remarks>Since couchdb.net uses flurl and that uses JSON.NET the int value json will be converted to string.</remarks>
        public static View<string, string, TRole> Role { get; set; }

        /// <summary>
        /// Key = NormalizedName, Value = Rev
        /// </summary>
        public static View<string, string, TRole> RoleNormalizedName { get; set; }

        /// <summary>
        /// Apply these options to the existing view properties.
        /// </summary>
        /// <param name="options"></param>
        public static void ApplyOptions(CouchDbViewOptions options)
        {
            Check.NotNull(options, nameof(options));

            Document = options.Document;
            User = new(Document, options.User);
            UserNormalizedUsername = new(Document, options.UserNormalizedUsername);
            UserNormalizedEmail = new(Document, options.UserNormalizedEmail);
            UserRolesNormalizedName = new(Document, options.UserRolesNormalizedName);
            UserClaims = new(Document, options.UserClaims);
            UserLogins = new(Document, options.UserLogins);
            Role = new(Document, options.Role);
            RoleNormalizedName = new(Document, options.RoleNormalizedName);
        }
    }
}