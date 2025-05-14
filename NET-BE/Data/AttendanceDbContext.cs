using Microsoft.EntityFrameworkCore;
using NET_BE.Model;

namespace NET_BE.Data
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Class> Classes => Set<Class>();
        public DbSet<Lecturer> Lecturers => Set<Lecturer>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<Grade> Grades => Set<Grade>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ClassSubject
            modelBuilder.Entity<ClassSubject>()
                .HasOne(cs => cs.Class)
                .WithMany(c => c.ClassSubjects)
                .HasForeignKey(cs => cs.ClassId);

            modelBuilder.Entity<ClassSubject>()
                .HasOne(cs => cs.Subject)
                .WithMany(s => s.ClassSubjects)
                .HasForeignKey(cs => cs.SubjectId);

            // Schedule
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.ClassSubject)
                .WithMany(cs => cs.Schedules)
                .HasForeignKey(s => s.ClassSubjectId);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Lecturer)
                .WithMany(l => l.Schedules)
                .HasForeignKey(s => s.LecturerId);

            // Attendance
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Schedule)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.ScheduleId);

            // Grade
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany()
                .HasForeignKey(g => g.StudentId);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.ClassSubject)
                .WithMany()
                .HasForeignKey(g => g.ClassSubjectId);
        }

    }
}
