using NET_BE.Model;

namespace NET_BE.DTOs
{
    public class AttendanceDto
    {
        public string AttendanceId { get; set; }
        public string StudentId { get; set; }
        public string ScheduleId { get; set; }
        public AttendanceStatus Status { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class AttendanceCreateDto
    {
        public string StudentId { get; set; }
        public string ScheduleId { get; set; }
        public AttendanceStatus Status { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class AttendanceUpdateDto
    {
        public AttendanceStatus Status { get; set; }
        public DateTime DateTime { get; set; }
    }
}
