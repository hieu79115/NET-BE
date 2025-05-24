using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NET_BE.DTOs;
using NET_BE.Model;
using NET_BE.Repositories;

namespace NET_BE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IRepository<Attendance> _repository;

        public AttendanceController(IRepository<Attendance> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? studentId,
            [FromQuery] string? scheduleId
        )
        {
            var attendances = await _repository.GetAllAsync();
            if (!string.IsNullOrEmpty(studentId))
                attendances = attendances.Where(a => a.StudentId == studentId);
            if (!string.IsNullOrEmpty(scheduleId))
                attendances = attendances.Where(a => a.ScheduleId == scheduleId);

            var result = attendances
                .Select(a => new AttendanceDto
                {
                    AttendanceId = a.AttendanceId,
                    StudentId = a.StudentId,
                    ScheduleId = a.ScheduleId,
                    Status = a.Status,
                    DateTime = a.DateTime,
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var attendance = await _repository.GetByIdAsync(id);
            if (attendance == null)
                return NotFound();
            var dto = new AttendanceDto
            {
                AttendanceId = attendance.AttendanceId,
                StudentId = attendance.StudentId,
                ScheduleId = attendance.ScheduleId,
                Status = attendance.Status,
                DateTime = attendance.DateTime,
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AttendanceCreateDto dto)
        {
            var attendance = new Attendance
            {
                AttendanceId = Guid.NewGuid().ToString(),
                StudentId = dto.StudentId,
                ScheduleId = dto.ScheduleId,
                Status = dto.Status,
                DateTime = dto.DateTime,
            };
            await _repository.AddAsync(attendance);
            return Ok(
                new AttendanceDto
                {
                    AttendanceId = attendance.AttendanceId,
                    StudentId = attendance.StudentId,
                    ScheduleId = attendance.ScheduleId,
                    Status = attendance.Status,
                    DateTime = attendance.DateTime,
                }            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AttendanceUpdateDto dto)
        {
            var attendance = await _repository.GetByIdAsync(id);
            if (attendance == null)
                return NotFound();

            attendance.Status = dto.Status;
            attendance.DateTime = dto.DateTime;
            await _repository.UpdateAsync(attendance);

            return Ok(
                new AttendanceDto
                {
                    AttendanceId = attendance.AttendanceId,
                    StudentId = attendance.StudentId,
                    ScheduleId = attendance.ScheduleId,
                    Status = attendance.Status,
                    DateTime = attendance.DateTime,
                }
            );
        }

        [HttpGet("by-schedule/{scheduleId}")]
        public async Task<IActionResult> GetBySchedule(string scheduleId)
        {
            var attendances = await _repository.GetAllAsync();
            
            // Let's include the student repository to get student names
            var studentRepository = HttpContext.RequestServices.GetRequiredService<IRepository<Student>>();
            var students = await studentRepository.GetAllAsync();
            
            var result = attendances
                .Where(a => a.ScheduleId == scheduleId)
                .Select(a => 
                {
                    var student = students.FirstOrDefault(s => s.StudentId == a.StudentId);
                    return new 
                    {
                        AttendanceId = a.AttendanceId,
                        StudentId = a.StudentId,
                        StudentName = student?.FullName,
                        ScheduleId = a.ScheduleId,
                        Status = a.Status,
                        DateTime = a.DateTime,
                    };
                })
                .ToList();

            return Ok(result);
        }
    }
}
