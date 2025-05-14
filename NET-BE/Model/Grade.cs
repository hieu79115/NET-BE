using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Grade
    {
        public int Id { get; set; }
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int ClassSubjectId { get; set; }

        [Required]
        [Range(0, 10)]
        public double Score { get; set; }

        public Student Student { get; set; }
        public ClassSubject ClassSubject { get; set; }
    }
}
