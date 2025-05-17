using Microsoft.EntityFrameworkCore;
using NET_BE.Model;

namespace NET_BE.Data
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Lecturer> Lecturers => Set<Lecturer>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<Attendance> Attendances => Set<Attendance>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ClassSubject - Subject
            modelBuilder.Entity<ClassSubject>()
                .HasOne(cs => cs.Subject)
                .WithMany(s => s.ClassSubjects)
                .HasForeignKey(cs => cs.SubjectId);

            // ClassSubject - Lecturer
            modelBuilder.Entity<ClassSubject>()
                .HasOne(cs => cs.Lecturer)
                .WithMany(l => l.ClassSubjects)
                .HasForeignKey(cs => cs.LecturerId);

            // Enrollment - Student
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);

            // Enrollment - ClassSubject
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.ClassSubject)
                .WithMany(cs => cs.Enrollments)
                .HasForeignKey(e => e.ClassSubjectId);

            // Schedule - ClassSubject
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.ClassSubject)
                .WithMany(cs => cs.Schedules)
                .HasForeignKey(s => s.ClassSubjectId)
                .OnDelete(DeleteBehavior.Cascade);
            // Schedule - Lecturer
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Lecturer)
                .WithMany(l => l.Schedules)
                .HasForeignKey(s => s.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);
            // Attendance - Student
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId);

            // Attendance - Schedule
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Schedule)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.ScheduleId);
        }

    }
}
