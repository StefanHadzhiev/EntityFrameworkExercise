using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }

        public StudentSystemContext([NotNull] DbContextOptions options)
            :base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.CONNECTION_STRING);
            }
            base.OnConfiguring(optionsBuilder);
        }

        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<Course> Courses { get; set; }

        public virtual DbSet<Homework> HomeworkSubmissions { get; set; }

        public virtual DbSet<Resource> Resources { get; set; }

        public virtual DbSet<StudentCourse> StudentCourses { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<StudentCourse>(e =>
            {
                e.HasKey(sc => new
                {
                    sc.StudentId,
                    sc.CourseId
                });
            });
        }
    }
}
