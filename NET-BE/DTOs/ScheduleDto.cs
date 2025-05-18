using System;

namespace NET_BE.DTOs
{
    public class ScheduleDto
    {
        public string ScheduleId { get; set; }
        public string ClassSubjectId { get; set; }
        public string LecturerId { get; set; }
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; }
    }

    public class ScheduleCreateUpdateDto
    {
        public string ClassSubjectId { get; set; }
        public string LecturerId { get; set; }
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; }
    }
}
