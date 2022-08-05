using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNIC.Models
{
    public class Course : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public byte[] Image { get; set; }

        // Relationships

        public virtual List<Section> Sections { get; set; }
        public virtual List<UserCourseResult> UserCourseResults { get; set; }

        public Quiz Quiz { get; set; }
        //public Guid QuizId { get; set; }
    }
}
