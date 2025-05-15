using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public enum AttendanceStatus
    {
        Present,       
        Absent,         
        ExcusedAbsence 
    }
    public class Attendance
    {
        public string AttendanceId { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public string ScheduleId { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        public DateTime DateTime { get; set; }

        public Student Student { get; set; }
        public Schedule Schedule { get; set; }
    }
}
