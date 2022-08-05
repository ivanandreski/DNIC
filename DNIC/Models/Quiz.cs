using System;
using System.Collections.Generic;

namespace DNIC.Models
{
    public class Quiz : BaseEntity
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public virtual List<Question> Questions { get; set; }
    }
}
