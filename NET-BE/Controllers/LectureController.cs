using Microsoft.AspNetCore.Mvc;
using NET_BE.DTOs;
using NET_BE.Model;
using NET_BE.Repositories;

namespace NET_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LectureController : Controller
    {
        private readonly IRepository<Lecturer> _repository;

        public LectureController(IRepository<Lecturer> repository)
        {
            _repository = repository;
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
            var updatedDto = new LecturerDto
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
            return Ok(new { Message = "Update successful", Lecturer = updatedDto });
        }
    }
}
