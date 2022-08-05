using System;

namespace DNIC.Models
{
    public class Answer : BaseEntity
    {
        public string Text { get; set; }

        public bool IsRight { get; set; }

        // Relationships

        public Guid QuestionId { get; set; }
        public Question Quiestion { get; set; }
    }
}
