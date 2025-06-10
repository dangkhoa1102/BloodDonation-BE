using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class BlogCreateDto
    {
        public Guid? AuthorId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; }
    }

    public class BlogUpdateDto
    {
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; }
    }

    public class BlogResponseDto
    {
        public Guid BlogId { get; set; }
        public Guid? AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? PublishDate { get; set; }
        public string Category { get; set; }
        public int ViewCount { get; set; }
    }

}
