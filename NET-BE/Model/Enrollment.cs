namespace NET_BE.Model
{
    public class Enrollment
    {
        public string EnrollmentId { get; set; }
        public string StudentId { get; set; }
        public string ClassSubjectId { get; set; }

        public Student Student { get; set; }
        public ClassSubject ClassSubject { get; set; }
    }
}
