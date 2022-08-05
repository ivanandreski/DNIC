using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNIC.Models
{
    public class UserCourseResult : BaseEntity
    {
        public string Username { get; set; }
        public User User { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public double Percentage { get; set; } = 0;

        [NotMapped]
        public bool Passed { get { return Percentage < 50 ? false : true; } }
    }
}
