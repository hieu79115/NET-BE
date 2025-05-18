namespace NET_BE.DTOs
{
    public class SubjectUpdateDto
    {
        public string Name { get; set; }
        public double FinalWeight { get; set; } = 0.6;
        public int Credits { get; set; } = 3;
    }
}
