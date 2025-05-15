using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Class
    {
        public string ClassId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ClassName { get; set; }

        public ICollection<ClassSubject> ClassSubjects { get; set; }
    }
}
