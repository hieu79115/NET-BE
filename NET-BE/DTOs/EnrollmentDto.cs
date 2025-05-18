namespace NET_BE.DTOs
{
    public class EnrollmentDto
    {
        public string EnrollmentId { get; set; }
        public string StudentId { get; set; }
        public string ClassSubjectId { get; set; }
        public double? MidtermScore { get; set; }
        public double? FinalScore { get; set; }
    }

    public class EnrollmentCreateDto
    {
        public string StudentId { get; set; }
        public string ClassSubjectId { get; set; }
    }

    public class EnrollmentScoreDto
    {
        public double? MidtermScore { get; set; }
        public double? FinalScore { get; set; }
    }
}
