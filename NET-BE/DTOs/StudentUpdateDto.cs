namespace NET_BE.DTOs
{
    public class StudentUpdateDto
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
    }
}
