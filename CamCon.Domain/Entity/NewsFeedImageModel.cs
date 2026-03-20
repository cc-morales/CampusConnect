using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamCon.Domain.Entity
{
    [Table("NewsFeedImages")]
    [PrimaryKey("NewsFeedImageId")]
    public class NewsFeedImageModel
    {
        public Guid NewsFeedImageId { get; set; }

        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        public Guid NewsFeedId { get; set; }
    }
}
