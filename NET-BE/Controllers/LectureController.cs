using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET_BE.DTOs;
using NET_BE.Model;
using NET_BE.Repositories;
using System.Linq;
using System.Security.Claims;

namespace NET_BE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LectureController : Controller
    {
        private readonly IRepository<Lecturer> _repository;
        private readonly IRepository<ClassSubject> _classSubjectRepository;
        private readonly IRepository<Schedule> _scheduleRepository;
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Attendance> _attendanceRepository;
        private readonly IRepository<Subject> _subjectRepository;

        public LectureController(
            IRepository<Lecturer> repository,
            IRepository<ClassSubject> classSubjectRepository,
            IRepository<Schedule> scheduleRepository,
            IRepository<Enrollment> enrollmentRepository,
            IRepository<Student> studentRepository,
            IRepository<Attendance> attendanceRepository,
            IRepository<Subject> subjectRepository
        )
        {
            _repository = repository;
            _classSubjectRepository = classSubjectRepository;
            _scheduleRepository = scheduleRepository;
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _attendanceRepository = attendanceRepository;
            _subjectRepository = subjectRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _repository.GetPagedAsync(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LecturerDto>> GetLecturer(string id)
        {
            var lecturer = await _repository.GetByIdAsync(id);
            if (lecturer == null) return NotFound("Lecturer not found");

            var dto = new LecturerDto
            {
                LecturerId = lecturer.LecturerId,
                FullName = lecturer.FullName,
                Email = lecturer.Email,
                PhoneNumber = lecturer.PhoneNumber,
                Gender = lecturer.Gender,
                DateOfBirth = lecturer.DateOfBirth,
                Department = lecturer.Department,
                AcademicTitle = lecturer.AcademicTitle,
                Degree = lecturer.Degree,
            };

            return Ok(new { Message = "Successful", Lecturer = dto });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLecturer(string id, [FromBody] LecturerUpdateDto dto)
        {
            var lecturer = await _repository.GetByIdAsync(id);
            if (lecturer == null) return NotFound("Lecturer not found");

            lecturer.FullName = dto.FullName;
            lecturer.Email = dto.Email;
            lecturer.PhoneNumber = dto.PhoneNumber;
            lecturer.Gender = dto.Gender;
            lecturer.DateOfBirth = dto.DateOfBirth;
            lecturer.Department = dto.Department;
            lecturer.AcademicTitle = dto.AcademicTitle;
            lecturer.Degree = dto.Degree;

            await _repository.UpdateAsync(lecturer);

            return Ok(new { Message = "Update successful" });
        }

        [HttpGet("class-subjects")]
        public async Task<IActionResult> GetClassSubjects()
        {
            string lecturerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(lecturerId))
                return Unauthorized("Invalid token");

            var classSubjects = await _classSubjectRepository.GetAllAsync();
            var subjects = await _subjectRepository.GetAllAsync();

            var result = classSubjects
                .Where(cs => cs.LecturerId == lecturerId)
                .Select(cs => new
                {
                    cs.ClassSubjectId,
                    SubjectName = subjects.FirstOrDefault(s => s.SubjectId == cs.SubjectId)?.Name,
                });

            return Ok(result);
        }

        [HttpGet("schedules")]
        public async Task<IActionResult> GetSchedules()
        {
            string lecturerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(lecturerId))
                return Unauthorized("Invalid token");

            var schedules = await _scheduleRepository.GetAllAsync();
            var classSubjects = await _classSubjectRepository.GetAllAsync();
            var subjects = await _subjectRepository.GetAllAsync();

            var result = schedules
                .Where(s => s.LecturerId == lecturerId)
                .Select(s =>
                {
                    var cs = classSubjects.FirstOrDefault(c => c.ClassSubjectId == s.ClassSubjectId);
                    var subject = subjects.FirstOrDefault(sub => sub.SubjectId == cs?.SubjectId);
                    return new
                    {
                        s.ScheduleId,
                        s.Date,
                        s.TimeSlot,
                        cs?.ClassSubjectId,
                        SubjectName = subject?.Name,
                    };
                });

            return Ok(result);
        }

        [HttpGet("class-subjects/{classSubjectId}/students")]
        public async Task<IActionResult> GetStudentsByClassSubject(string classSubjectId)
        {
            var enrollments = await _enrollmentRepository.GetAllAsync();
            var studentIds = enrollments
                .Where(e => e.ClassSubjectId == classSubjectId)
                .Select(e => e.StudentId)
                .ToList();

            var students = await _studentRepository.GetAllAsync();
            var result = students
                .Where(s => studentIds.Contains(s.StudentId))
                .Select(s => new
                {
                    s.StudentId,
                    s.FullName,
                    s.Email,
                    s.Phone,
                });

            return Ok(result);
        }

        [HttpPost("attendance")]
        public async Task<IActionResult> MarkAttendance([FromBody] AttendanceDto dto)
        {
            if (dto == null || dto.StudentId == null || dto.ScheduleId == null)
                return BadRequest("Invalid data");

            var attendance = new Attendance
            {
                AttendanceId = Guid.NewGuid().ToString(),
                StudentId = dto.StudentId,
                ScheduleId = dto.ScheduleId,
                Status = dto.Status,
                DateTime = DateTime.Now
            };

            await _attendanceRepository.AddAsync(attendance);
            return Ok("Attendance recorded successfully");
        }
    }
}
