using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamCon.Domain.Entity
{
    [Table("NewsFeedComments")]
    [PrimaryKey("NewsFeedCommentId")]
    public class NewsFeedCommentModel
    {
        public Guid NewsFeedCommentId { get; set; }

        public string? Id { get; set; }

        [ForeignKey("Id")]
        public ApplicationUserModel? User { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<NewsFeedCommentModel> Replies { get; set; } = new();

        //Current Post
        public Guid NewsFeedId { get; set; }
        //Replied Comments if null root
        public Guid? ParentCommentId { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsFlagged { get; set; }
    }
}
