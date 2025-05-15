using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Schedule
    {
        public string ScheduleId { get; set; }
        [Required]
        public string ClassSubjectId { get; set; }

        [Required]
        public string LecturerId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(20)]
        public string TimeSlot { get; set; }

        public ClassSubject ClassSubject { get; set; }
        public Lecturer Lecturer { get; set; }

        public ICollection<Attendance> Attendances { get; set; }
    }
}
