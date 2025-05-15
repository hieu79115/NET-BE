using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Lecturer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        public ICollection<Schedule> Schedules { get; set; }
    }
}
