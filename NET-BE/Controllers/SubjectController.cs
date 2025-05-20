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
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly IRepository<Subject> _repository;

        public SubjectController(IRepository<Subject> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subjects = await _repository.GetAllAsync();
            var dtos = subjects.Select(s => new SubjectDto
            {
                SubjectId = s.SubjectId,
                Name = s.Name,
                FinalWeight = s.FinalWeight,
                Credits = s.Credits
            }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var subject = await _repository.GetByIdAsync(id);
            if (subject == null) return NotFound();

            var dto = new SubjectDto
            {
                SubjectId = subject.SubjectId,
                Name = subject.Name,
                FinalWeight = subject.FinalWeight,
                Credits = subject.Credits
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubjectDto dto)
        {
            var subject = new Subject
            {
                SubjectId = dto.SubjectId,
                Name = dto.Name,
                FinalWeight = dto.FinalWeight,
                Credits = dto.Credits
            };
            await _repository.AddAsync(subject);

            var result = new SubjectDto
            {
                SubjectId = subject.SubjectId,
                Name = subject.Name,
                FinalWeight = subject.FinalWeight,
                Credits = subject.Credits
            };
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SubjectUpdateDto dto)
        {
            var subject = await _repository.GetByIdAsync(id);
            if (subject == null) return NotFound();

            subject.Name = dto.Name;
            subject.FinalWeight = dto.FinalWeight;
            subject.Credits = dto.Credits;

            await _repository.UpdateAsync(subject);

            var result = new SubjectDto
            {
                SubjectId = subject.SubjectId,
                Name = subject.Name,
                FinalWeight = subject.FinalWeight,
                Credits = subject.Credits
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var subject = await _repository.GetByIdAsync(id);
            if (subject == null) return NotFound();
            await _repository.DeleteAsync(id);
            return Ok("Deleted successfully");
        }
    }
}
