using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DNIC.Models
{
    public class Question : BaseEntity
    {
        [Required]
        public string Text { get; set; }

        // Relationships

        public Guid QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public virtual List<Answer> Answers { get; set; }
    }
}
