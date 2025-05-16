using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Lecturer
    {
        public string LecturerId { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = default!;

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(50)]
        public string? AcademicTitle { get; set; }

        [MaxLength(50)]
        public string? Degree { get; set; }

        public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
