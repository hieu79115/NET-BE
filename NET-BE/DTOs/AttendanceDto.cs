using NET_BE.Model;

namespace NET_BE.DTOs
{
    public class AttendanceDto
    {
        public string StudentId { get; set; }
        public string ScheduleId { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
