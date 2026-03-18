using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamCon.Domain
{
    public class PaginationRequestModel
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public Guid? MyOrganizationId { get; set; }
    }
}
