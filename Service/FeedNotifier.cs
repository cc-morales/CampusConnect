using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class FeedNotifier : IFeedNotifier
    {
        public async Task OnOrganizationClick(Guid? gui)
        {
            if (this != null)
                await this.OnOrganizationClick(gui);
        }
    }
}
