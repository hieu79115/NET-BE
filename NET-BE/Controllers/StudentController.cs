using Microsoft.AspNetCore.Mvc;
using NET_BE.DTOs;
using NET_BE.Model;
using NET_BE.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NET_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IRepository<Student> _repository;

        public StudentController(IRepository<Student> repository)
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
        public async Task<ActionResult<StudentDto>> GetStudent(string id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return NotFound();

            var dto = new StudentDto
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                Email = student.Email,
                Phone = student.Phone,
                Address = student.Address
            };

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] StudentUpdateDto dto)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return NotFound();

            student.FullName = dto.FullName;
            student.DateOfBirth = dto.DateOfBirth;
            student.Phone = dto.Phone;
            student.Address = dto.Address;
            student.Password = dto.Password;

            await _repository.UpdateAsync(student);
            return Ok(new { Message = "Update successful", Student = student });
        }
    }
}
