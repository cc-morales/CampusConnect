using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamCon.Domain
{
    public class Enums
    {
        public enum RequestType
        {
            GET,
            POST,
            DELETE,
            PUT
        }
        public enum OrganizationType
        {
            Department,
            Organization
        }
        public enum PageRequestStatus
        {
            Pending,
            Approved,
            Rejected
        }

        public enum NotificationType {
            PageRequest,
            Comment,
            Post

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
