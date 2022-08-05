using DNIC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNIC.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                    : base(options)
        {
        }

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<UserCourseResult> UserCourseResults { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Quiz> Quizes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserCourseResult>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserCourseResults)
                .HasForeignKey(x => x.Username);

            builder.Entity<UserCourseResult>()
                .HasOne(x => x.Course)
                .WithMany(x => x.UserCourseResults)
                .HasForeignKey(x => x.CourseId);

            builder.Entity<Answer>()
                .HasOne(x => x.Quiestion)
                .WithMany(x => x.Answers)
                .HasForeignKey(x => x.QuestionId);

            builder.Entity<Question>()
                .HasOne(x => x.Quiz)
                .WithMany(x => x.Questions)
                .HasForeignKey(x => x.QuizId);

            builder.Entity<Course>()
                .HasOne(x => x.Quiz)
                .WithOne(x => x.Course)
                .HasForeignKey<Quiz>(x => x.CourseId);
        }
    }
}
