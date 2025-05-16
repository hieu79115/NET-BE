using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace NET_BE.Model
{
    public class ClassSubject
    {
        public string ClassSubjectId { get; set; }

        [Required]
        public string LecturerId { get; set; }

        [Required]
        public string SubjectId { get; set; }


        public Subject Subject { get; set; }
        public Lecturer Lecturer { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
    }
}
