namespace AspNetCore.Identity.CouchDB
{
    public record CouchDbViewOptions
    {
        /// <summary>
        /// Initialize a new <see cref="CouchDbViewOptions"/> with default values.
        /// </summary>
        public CouchDbViewOptions()
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
        }

        /// <summary>
        /// The name of the design document that will contain the views.
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        /// The name of the user view. <br/>
        /// With reduce = true (default) then Key = null, Value = 'count'<br/>
        /// When reduce = false (manual) then Key = Id, Value = Rev
        /// </summary>
        /// <remarks>Since couchdb.net uses flurl and that uses JSON.NET the int value json will be converted to string.</remarks>
        public string User { get; set; }

        /// <summary>
        /// The name of the user.normalized_name view. <br/>
        /// Key = NormalizedUserName, Value = Rev
        /// </summary>
        public string UserNormalizedUsername { get; set; }

        /// <summary>
        /// The name of the user.normalized_email view. <br/>
        /// Key = NormalizedEmail, Value = Rev
        /// </summary>
        public string UserNormalizedEmail { get; set; }

        /// <summary>
        /// The name of the user.roles.normalized_name view. <br/>
        /// Key = Role.NormalizedName, Value = Rev
        /// </summary>
        public string UserRolesNormalizedName { get; set; }

        /// <summary>
        /// The name of the user.claims view. <br/>
        /// Key = [Type, Value], Value = Rev
        /// </summary>
        public string UserClaims { get; set; }

        /// <summary>
        /// The logins of the user.logins view. <br/>
        /// Key = [LoginProvider, ProviderKey], Value = Rev
        /// </summary>
        public string UserLogins { get; set; }

        /// <summary>
        /// The name of the role view. <br/>
        /// With reduce = true (default) then Key = null, Value = 'count'<br/>
        /// When reduce = false (manual) then Key = Id, Value = Rev
        /// </summary>
        /// <remarks>Since couchdb.net uses flurl and that uses JSON.NET the int value json will be converted to string.</remarks>
        public string Role { get; set; }

        /// <summary>
        /// The name of the role.normalized_name view. <br/>
        /// Key = NormalizedName, Value = Rev
        /// </summary>
        public string RoleNormalizedName { get; set; }
    }
}