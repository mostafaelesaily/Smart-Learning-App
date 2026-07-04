using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smart_Learning_App.Data.Models;

namespace Smart_Learning_App.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lessons> Lessons { get; set; }
        public DbSet<Topics> Topics { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Enrollments> Enrollments { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<InstructorRequest> instructorRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Course Configuration Start
            modelBuilder.Entity<Course>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Course>()
                .Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<Course>()
                .Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1000);
            modelBuilder.Entity<Course>()
                .HasIndex(x => x.Title)
                .IsUnique(true);
            // Course Configuration End

            // Lessons Configuration Start
            modelBuilder.Entity<Lessons>()
                .HasKey(l => l.Id);
            modelBuilder.Entity<Lessons>()
                .Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<Lessons>()
                .Property(l => l.OrderIndex)
                .IsRequired();
            modelBuilder.Entity<Lessons>()
                .HasIndex(l => l.courseId);
            modelBuilder.Entity<Lessons>()
                .HasIndex(l => l.Title);
            // Lessons Configuration End

            // Topics Configuration Start
            modelBuilder.Entity<Topics>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<Topics>()
                .Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<Topics>()
                .Property(t => t.Description)
                .HasMaxLength(1000);
            modelBuilder.Entity<Topics>()
                .HasIndex(t => t.LessonId);
            // Topics Configuration End

            // Progress Configuration Start
            modelBuilder.Entity<Progress>()
                .HasKey(p => p.id);
            modelBuilder.Entity<Progress>()
                .Property(p => p.UserId)
                .IsRequired();
            modelBuilder.Entity<Progress>()
                .Property(p => p.IsCompleted)
                .IsRequired();
            modelBuilder.Entity<Progress>()
                .HasIndex(p => new { p.UserId, p.LessonId })
                .IsUnique(true);
            // Progress Configuration End

            // Enrollments Configuration Start
            modelBuilder.Entity<Enrollments>()
                .HasKey(e => e.id);
            modelBuilder.Entity<Enrollments>()
                .Property(e => e.UserId)
                .IsRequired();
            modelBuilder.Entity<Enrollments>()
                .HasIndex(e => new { e.CourseId, e.UserId })
                .IsUnique(true);
            // Enrollments Configuration End 

            // Files Configuration Start
              modelBuilder.Entity<Files>()
                .HasKey(f => f.id);
            modelBuilder.Entity<Files>()
                .Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(255);
                
            modelBuilder.Entity<Files>()
                .Property(x => x.FilePath)
                .IsRequired()
                .HasMaxLength(500);
            modelBuilder.Entity<Files>()
                .Property(x => x.FileType)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Files>()
                .HasIndex(f => f.LessonId);
            modelBuilder.Entity<Files>()
                .HasIndex(f => f.FileName);
            // File Configuration End

            // Relations Start 
            // Course - Lessons Start 
            modelBuilder.Entity<Lessons>()
                .HasOne(x => x.Course)
                .WithMany(x => x.Lessons)
                .HasForeignKey(x => x.courseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course - Lessons End

            // Lessons - Topics Start
            modelBuilder.Entity<Topics>()
                .HasOne(t => t.lessons)
                .WithMany(l => l.Topics)
                .HasForeignKey(t => t.LessonId)
                .OnDelete(DeleteBehavior.Restrict);
            // Lessons - Topics End

            // Lessons - Progress Start
            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Lesson)
                .WithMany(l => l.Progresses)
                .HasForeignKey(p => p.LessonId)
                .OnDelete(DeleteBehavior.Restrict);
            // Lessons - Progress End

            // User - Progress Start
            modelBuilder.Entity<Progress>()
                .HasOne(p => p.User)
                .WithMany(u => u.Progresses)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // User - Progress End

            // Course - Enrollments Start
            modelBuilder.Entity<Enrollments>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            // Course - Enrollments End

            // User - Enrollments Start
            modelBuilder.Entity<Enrollments>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // User - Enrollments End

            // Course - File Start 
            modelBuilder.Entity<Files>()
                .HasOne(f => f.lessons)
                .WithMany(c => c.Files)
                .HasForeignKey(f => f.LessonId)
                .OnDelete(DeleteBehavior.Restrict);
            // Course - File End

            // User_Course Start : 
            modelBuilder.Entity<Course>()
           .HasOne(c => c.Instructor)
           .WithMany(u => u.Courses)
           .HasForeignKey(c => c.InstructorId)
           .OnDelete(DeleteBehavior.Restrict);
            // User_Course End.

            // User_InstructorRequest Start : 
            modelBuilder.Entity<InstructorRequest>()
                .HasOne(u => u.user)
                .WithMany(i => i.instructorRequests)
                .HasForeignKey(u => u.userId)
                .OnDelete(DeleteBehavior.Restrict);
            // User_InstructorRequest End.

            // Relations End
        }
    }
}
