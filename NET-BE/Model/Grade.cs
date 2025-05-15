using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Grade
    {
        public string GradeId { get; set; }
        [Required]
        public string StudentId { get; set; }

        [Required]
        public string ClassSubjectId { get; set; }

        [Required]
        [Range(0, 10)]
        public double Score { get; set; }

        public Student Student { get; set; }
        public ClassSubject ClassSubject { get; set; }
    }
}
