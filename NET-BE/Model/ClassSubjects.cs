using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace NET_BE.Model
{
    public class ClassSubject
    {
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        public Class Class { get; set; }
        public Subject Subject { get; set; }

        public ICollection<Schedule> Schedules { get; set; }
    }
}
