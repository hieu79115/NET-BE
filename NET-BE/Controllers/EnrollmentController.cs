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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IRepository<Enrollment> _repository;

        public EnrollmentController(IRepository<Enrollment> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _repository.GetAllAsync();
            var result = enrollments.Select(e => new EnrollmentDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                ClassSubjectId = e.ClassSubjectId,
                MidtermScore = e.MidtermScore,
                FinalScore = e.FinalScore
            }).ToList();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] EnrollmentCreateDto dto)
        {
            var enrollment = new Enrollment
            {
                EnrollmentId = Guid.NewGuid().ToString(),
                StudentId = dto.StudentId,
                ClassSubjectId = dto.ClassSubjectId
            };
            await _repository.AddAsync(enrollment);
            return Ok(new EnrollmentDto
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                ClassSubjectId = enrollment.ClassSubjectId
            });
        }

        [HttpPut("{id}/scores")]
        public async Task<IActionResult> UpdateScores(string id, [FromBody] EnrollmentScoreDto dto)
        {
            var enrollment = await _repository.GetByIdAsync(id);
            if (enrollment == null) return NotFound();

            enrollment.MidtermScore = dto.MidtermScore;
            enrollment.FinalScore = dto.FinalScore;
            await _repository.UpdateAsync(enrollment);

            return Ok(new EnrollmentDto
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                ClassSubjectId = enrollment.ClassSubjectId,
                MidtermScore = enrollment.MidtermScore,
                FinalScore = enrollment.FinalScore
            });
        }

        [HttpGet("by-student/{studentId}")]
        public async Task<IActionResult> GetByStudent(string studentId)
        {
            var enrollments = await _repository.GetAllAsync();
            var result = enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentDto
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    ClassSubjectId = e.ClassSubjectId,
                    MidtermScore = e.MidtermScore,
                    FinalScore = e.FinalScore
                }).ToList();
            return Ok(result);
        }

        [HttpGet("by-class/{classSubjectId}")]
        public async Task<IActionResult> GetByClass(string classSubjectId)
        {
            var enrollments = await _repository.GetAllAsync();
            var result = enrollments
                .Where(e => e.ClassSubjectId == classSubjectId)
                .Select(e => new EnrollmentDto
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    ClassSubjectId = e.ClassSubjectId,
                    MidtermScore = e.MidtermScore,
                    FinalScore = e.FinalScore
                }).ToList();
            return Ok(result);
        }
    }
}
