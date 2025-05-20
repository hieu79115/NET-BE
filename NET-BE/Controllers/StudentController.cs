using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET_BE.DTOs;
using NET_BE.Model;
using NET_BE.Repositories;

namespace NET_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IRepository<Student> _repository;
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly IRepository<Schedule> _scheduleRepository;
        private readonly IRepository<Attendance> _attendanceRepository;
        private readonly IRepository<ClassSubject> _classSubjectRepository;
        private readonly IRepository<Subject> _subjectRepository;
        private readonly IRepository<Lecturer> _lecturerRepository;

        public StudentController(
            IRepository<Student> repository,
            IRepository<Enrollment> enrollmentRepository,
            IRepository<Schedule> scheduleRepository,
            IRepository<Attendance> attendanceRepository,
            IRepository<ClassSubject> classSubjectRepository,
            IRepository<Subject> subjectRepository,
            IRepository<Lecturer> lectureRepository
        )
        {
            _repository = repository;
            _enrollmentRepository = enrollmentRepository;
            _scheduleRepository = scheduleRepository;
            _attendanceRepository = attendanceRepository;
            _classSubjectRepository = classSubjectRepository;
            _subjectRepository = subjectRepository;
            _lecturerRepository = lectureRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var result = await _repository.GetPagedAsync(pageIndex, pageSize);

            var dtoResult = new PagedModel<StudentDto>(
                result.TotalCount,
                result.Data.Select(student => new StudentDto
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    DateOfBirth = student.DateOfBirth,
                    Email = student.Email,
                    Phone = student.Phone,
                    Address = student.Address,
                }),
                result.PageIndex,
                result.PageSize
            );

            return Ok(dtoResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(string id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            var dto = new StudentDto
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                Email = student.Email,
                Phone = student.Phone,
                Address = student.Address,
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Student")]
        [HttpPut("{studentId}")]
        public async Task<IActionResult> UpdateStudent(
            string studentId,
            [FromBody] StudentUpdateDto dto
        )
        {
            var student = await _repository.GetByIdAsync(studentId);
            if (student == null)
                return NotFound("Student not found");

            student.FullName = dto.FullName;
            student.DateOfBirth = dto.DateOfBirth;
            student.Phone = dto.Phone;
            student.Address = dto.Address;

            await _repository.UpdateAsync(student);
            return Ok("Update student successful");
        }

        [HttpDelete("{studentId}")]
        public async Task<IActionResult> DeleteStudent(string studentId)
        {
            var student = await _repository.GetByIdAsync(studentId);
            if (student == null)
                return NotFound("Student not found");

            await _repository.DeleteAsync(studentId);
            return Ok("Detele student successful");
        }

        [HttpGet("class-subjects")]
        public async Task<IActionResult> GetEnrolledClassSubjects()
        {
            string studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized("Invalid token");

            var enrollments = await _enrollmentRepository.GetAllAsync();
            var classSubjectIds = enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.ClassSubjectId)
                .ToList();

            var classSubjects = await _classSubjectRepository.GetAllAsync();
            var subjects = await _subjectRepository.GetAllAsync();

            var result = classSubjects
                .Where(cs => classSubjectIds.Contains(cs.ClassSubjectId))
                .Select(cs => new EnrolledClassSubjectDto
                {
                    ClassSubjectId = cs.ClassSubjectId,
                    SubjectName = subjects.FirstOrDefault(s => s.SubjectId == cs.SubjectId)?.Name,
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("schedules")]
        public async Task<IActionResult> GetSchedulesByStudent()
        {
            string studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized("Invalid token");

            var enrollments = await _enrollmentRepository.GetAllAsync();
            var classSubjectIds = enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.ClassSubjectId)
                .ToList();

            var schedules = await _scheduleRepository.GetAllAsync();
            var classSubjects = await _classSubjectRepository.GetAllAsync();
            var subjects = await _subjectRepository.GetAllAsync();
            var lecturers = await _lecturerRepository.GetAllAsync();

            var studentSchedules = schedules
                .Where(s => classSubjectIds.Contains(s.ClassSubjectId))
                .Select(s =>
                {
                    var classSubject = classSubjects.FirstOrDefault(cs =>
                        cs.ClassSubjectId == s.ClassSubjectId
                    );
                    var subject = subjects.FirstOrDefault(sub =>
                        sub.SubjectId == classSubject?.SubjectId
                    );
                    var lecturer = lecturers.FirstOrDefault(l => l.LecturerId == s.LecturerId);

                    return new
                    {
                        s.ScheduleId,
                        s.Date,
                        s.TimeSlot,
                        classSubject?.ClassSubjectId,
                        SubjectName = subject?.Name,
                        LecturerName = lecturer?.FullName,
                    };
                })
                .ToList();

            return Ok(studentSchedules);
        }

        [HttpGet("attendances")]
        public async Task<IActionResult> GetStudentAttendances()
        {
            string studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized("Invalid token");

            var attendances = await _attendanceRepository.GetAllAsync();
            var schedules = await _scheduleRepository.GetAllAsync();
            var classSubjects = await _classSubjectRepository.GetAllAsync();
            var subjects = await _subjectRepository.GetAllAsync();

            var studentAttendances = attendances
                .Where(a => a.StudentId == studentId)
                .Select(a =>
                {
                    var schedule = schedules.FirstOrDefault(s => s.ScheduleId == a.ScheduleId);
                    var classSubject = classSubjects.FirstOrDefault(cs =>
                        cs.ClassSubjectId == schedule?.ClassSubjectId
                    );
                    var subject = subjects.FirstOrDefault(sub =>
                        sub.SubjectId == classSubject?.SubjectId
                    );

                    return new
                    {
                        a.ScheduleId,
                        a.Status,
                        a.DateTime,
                        schedule?.Date,
                        schedule?.TimeSlot,
                        classSubject?.ClassSubjectId,
                        SubjectName = subject?.Name,
                    };
                })
                .ToList();

            return Ok(studentAttendances);
        }
    }
}
