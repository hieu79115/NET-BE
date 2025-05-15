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
        public int Id { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public int ScheduleId { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        public DateTime CheckInTime { get; set; }

        public Student Student { get; set; }
        public Schedule Schedule { get; set; }
    }
}
