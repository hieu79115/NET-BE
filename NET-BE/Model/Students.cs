using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string StudentCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        public ICollection<Attendance> Attendances { get; set; }

    }
}
