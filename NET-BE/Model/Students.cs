using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Student
    {
        public string StudentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        public ICollection<Attendance> Attendances { get; set; }

    }
}
