using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Subject
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<ClassSubject> ClassSubjects { get; set; }
    }
}
