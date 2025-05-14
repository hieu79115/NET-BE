using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int ScheduleId { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        [Required]
        public DateTime CheckInTime { get; set; }

        public Student Student { get; set; }
        public Schedule Schedule { get; set; }
    }
}
