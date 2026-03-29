namespace Domain
{
    public class Enums
    {
        public enum RequestType
        {
            GET,
            POST,
            DELETE,
            PUT,
            PATCH
        }
        public enum Action
        {
            Create,
            Update,
            Delete,
            None
        }
        public enum OrganizationType
        {
            Department,
            Organization
        }
        public enum NotificationType
        {
            PageRequest,
            Comment,
            Post
        }
        public enum PageRequestStatus
        {
            Pending,
            Approved,
            Rejected
        }

        public enum LikeType
        {
            Like,
            Heart,
            Funny,
            Wow,
            Angry,
            Cry
        }

        public enum PageRequestType
        {
            PageRequest,
            Contributor
        }
    }
}
