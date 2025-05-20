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
    public class ClassSubjectController : ControllerBase
    {
        private readonly IRepository<ClassSubject> _repository;

        public ClassSubjectController(IRepository<ClassSubject> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassSubjectDto dto)
        {
            var entity = new ClassSubject
            {
                ClassSubjectId = dto.ClassSubjectId,
                LecturerId = dto.LecturerId,
                SubjectId = dto.SubjectId
            };
            await _repository.AddAsync(entity);
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ClassSubjectDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            entity.LecturerId = dto.LecturerId;
            entity.SubjectId = dto.SubjectId;
            await _repository.UpdateAsync(entity);
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            await _repository.DeleteAsync(id);
            return Ok("Deleted successfully");
        }
    }
}