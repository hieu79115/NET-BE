using System.ComponentModel.DataAnnotations;

namespace NET_BE.DTOs
{
    public class LecturerDto
    {
        public string LecturerId { get; set; }
        public string FullName { get; set; } 
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Department { get; set; }
        public string? AcademicTitle { get; set; }

        public string? Degree { get; set; }

    }
}
