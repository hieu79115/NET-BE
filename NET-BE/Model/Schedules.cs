using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Schedule
    {
        public int Id { get; set; }
        [Required]
        public int ClassSubjectId { get; set; }

        [Required]
        public int LecturerId { get; set; }

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
