using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNIC.Models
{
    public class Section : BaseEntity
    {
        public int Page { get; set; }

        public string Title { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Text { get; set; }

        // Relationships

        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}
