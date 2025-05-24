using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET_BE.Model;
using NET_BE.Repositories;
using NET_BE.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NET_BE.Controllers
{
    [Authorize(Roles = "Lecturer,Student")]
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IRepository<Schedule> _repository;
        private readonly IRepository<Enrollment> _enrollmentRepository;

        public ScheduleController(
            IRepository<Schedule> repository,
            IRepository<Enrollment> enrollmentRepository)
        {
            _repository = repository;
            _enrollmentRepository = enrollmentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedules = await _repository.GetAllAsync();
            var result = schedules.Select(s => new ScheduleDto
            {
                ScheduleId = s.ScheduleId,
                ClassSubjectId = s.ClassSubjectId,
                LecturerId = s.LecturerId,
                Date = s.Date,
                TimeSlot = s.TimeSlot
            }).ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var s = await _repository.GetByIdAsync(id);
            if (s == null) return NotFound();
            var dto = new ScheduleDto
            {
                ScheduleId = s.ScheduleId,
                ClassSubjectId = s.ClassSubjectId,
                LecturerId = s.LecturerId,
                Date = s.Date,
                TimeSlot = s.TimeSlot
            };
            return Ok(dto);
        }

        [HttpGet("by-class/{classSubjectId}")]
        public async Task<IActionResult> GetByClass(string classSubjectId)
        {
            var schedules = await _repository.GetAllAsync();
            var result = schedules
                .Where(s => s.ClassSubjectId == classSubjectId)
                .Select(s => new ScheduleDto
                {
                    ScheduleId = s.ScheduleId,
                    ClassSubjectId = s.ClassSubjectId,
                    LecturerId = s.LecturerId,
                    Date = s.Date,
                    TimeSlot = s.TimeSlot
                }).ToList();
            return Ok(result);
        }

        [HttpGet("by-lecturer/{lecturerId}")]
        public async Task<IActionResult> GetByLecturer(string lecturerId)
        {
            var schedules = await _repository.GetAllAsync();
            var result = schedules
                .Where(s => s.LecturerId == lecturerId)
                .Select(s => new ScheduleDto
                {
                    ScheduleId = s.ScheduleId,
                    ClassSubjectId = s.ClassSubjectId,
                    LecturerId = s.LecturerId,
                    Date = s.Date,
                    TimeSlot = s.TimeSlot
                }).ToList();
            return Ok(result);
        }

        [HttpGet("by-student/{studentId}")]
        public async Task<IActionResult> GetByStudent(string studentId)
        {
            var enrollments = await _enrollmentRepository.GetAllAsync();
            var classSubjectIds = enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.ClassSubjectId)
                .ToList();

            var schedules = await _repository.GetAllAsync();
            var result = schedules
                .Where(s => classSubjectIds.Contains(s.ClassSubjectId))
                .Select(s => new ScheduleDto
                {
                    ScheduleId = s.ScheduleId,
                    ClassSubjectId = s.ClassSubjectId,
                    LecturerId = s.LecturerId,
                    Date = s.Date,
                    TimeSlot = s.TimeSlot
                }).ToList();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScheduleCreateUpdateDto dto)
        {
            var schedule = new Schedule
            {
                ScheduleId = Guid.NewGuid().ToString(),
                ClassSubjectId = dto.ClassSubjectId,
                LecturerId = dto.LecturerId,
                Date = dto.Date,
                TimeSlot = dto.TimeSlot
            };
            await _repository.AddAsync(schedule);
            var result = new ScheduleDto
            {
                ScheduleId = schedule.ScheduleId,
                ClassSubjectId = schedule.ClassSubjectId,
                LecturerId = schedule.LecturerId,
                Date = schedule.Date,
                TimeSlot = schedule.TimeSlot
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ScheduleCreateUpdateDto dto)
        {
            var schedule = await _repository.GetByIdAsync(id);
            if (schedule == null) return NotFound();

            schedule.ClassSubjectId = dto.ClassSubjectId;
            schedule.LecturerId = dto.LecturerId;
            schedule.Date = dto.Date;
            schedule.TimeSlot = dto.TimeSlot;
            await _repository.UpdateAsync(schedule);

            return Ok(new ScheduleDto
            {
                ScheduleId = schedule.ScheduleId,
                ClassSubjectId = schedule.ClassSubjectId,
                LecturerId = schedule.LecturerId,
                Date = schedule.Date,
                TimeSlot = schedule.TimeSlot
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var schedule = await _repository.GetByIdAsync(id);
            if (schedule == null) return NotFound();
            await _repository.DeleteAsync(id);
            return Ok("Deleted successfully");
        }
    }
}
