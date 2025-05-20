using System;

namespace NET_BE.DTOs
{
    public class EnrollmentStudentDto
    {
        public EnrollmentDto Enrollment { get; set; }
        public StudentBasicInfoDto Student { get; set; }
    }

    public class StudentBasicInfoDto
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
