using AspNetCore.Identity.CouchDB.Models;

#pragma warning disable CA1034 // Nested types should not be visible

namespace AspNetCore.Identity.CouchDB.Internal
{
    /// <summary>
    /// Views regarding AspNetCore.Identity.CouchDb integration.
    /// </summary>
    /// <remarks>
    /// These can be overriden but make sure your database has the new views
    /// and that they perform the correct actions.
    /// </remarks>
    public static class Views
    {
        /// <summary>
        /// Views regarding a <see cref="CouchDbUser"/> document.
        /// </summary>
        public static class User<TUser, TRole>
            where TUser : CouchDbUser<TRole>
            where TRole : CouchDbRole
        {
            /// <summary>
            /// Key = NormalizedUserName, Value = Rev
            /// </summary>
            public static View<string, string, TUser> NormalizedUserName { get; set; } =
                View<string, string, TUser>.Create("user", "normalized_username");

            /// <summary>
            /// Key = NormalizedEmail, Value = Rev
            /// </summary>
            public static View<string, string, TUser> NormalizedEmail { get; set; } =
                View<string, string, TUser>.Create("user", "normalized_email");

            /// <summary>
            /// Key = Role, Value = Rev
            /// </summary>
            public static View<string, string, TUser> NormalizedRoleNames { get; set; } =
                View<string, string, TUser>.Create("user", "normalized_role_names");
        }

        /// <summary>
        /// Views regarding a <see cref="CouchDbRole"/> document.
        /// </summary>
        public static class Role<TRole>
            where TRole : CouchDbRole
        {
            /// <summary>
            /// Key = NormalizedName, Value = Rev
            /// </summary>
            /// <value>Default is user/normalized_name.</value>
            public static View<string, string, TRole> NormalizedName { get; set; } =
                View<string, string, TRole>.Create("role", "normalized_name");
        }
    }
}