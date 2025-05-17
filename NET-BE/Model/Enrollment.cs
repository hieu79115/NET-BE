namespace NET_BE.Model
{
    public class Enrollment
    {
        public string EnrollmentId { get; set; }
        public string StudentId { get; set; }
        public string ClassSubjectId { get; set; }
        public double? MidtermScore { get; set; }      
        public double? FinalScore { get; set; }
        public double FinalWeight { get; set; } = 0.6;
        public Student Student { get; set; }
        public ClassSubject ClassSubject { get; set; }
    }
}
