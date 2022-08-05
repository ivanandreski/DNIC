using System;
using System.ComponentModel.DataAnnotations;

namespace DNIC.Models.Dto
{
    public class SectionDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public Guid CourseId { get; set; }
    }
}
