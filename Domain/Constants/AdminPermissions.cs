namespace Domain.Constants
{
    /// <summary>
    /// Defines the available page-level permissions that can be assigned to admin accounts.
    /// These are used as authorization policy names throughout the Presentation layer.
    /// </summary>
    public static class AdminPermissions
    {
        public const string Dashboard = "Dashboard";
        public const string Organizations = "Organizations";
        public const string LiveNewsFeed = "LiveNewsFeed";
        public const string Sentiments = "Sentiments";
        public const string UserManagement = "UserManagement";
        public const string AccessControl = "AccessControl";

        /// <summary>
        /// Returns all available permission names. Useful for populating UI dropdowns.
        /// </summary>
        public static readonly string[] All =
        [
            Dashboard,
            Organizations,
            LiveNewsFeed,
            Sentiments,
            UserManagement,
            AccessControl
        ];
    }
}

