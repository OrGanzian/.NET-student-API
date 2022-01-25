using StudentAPI.Models;
using StudentAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private readonly int _maxValidAge;
        private readonly IDistributedCache _cache;
        private readonly IStudentRepository _studentRepository;

        public StudentsController(IStudentRepository studentRepository, IDistributedCache cache)
        {
            _cache = cache;
            _studentRepository = studentRepository;
            _maxValidAge = 18;
        }

        private Boolean isAgeNotValid(int ageToCheck)
        {
            return ageToCheck > _maxValidAge; 
        }

        [HttpGet]
        public async Task<IEnumerable<Student>> GetStudents()
        {
            var response = _cache.Get("getAll");
            if (response == null) // no cach avalable
            {
                var byteArr = await _studentRepository.Get();
                string json = MyConverter.SerializeJson<IEnumerable<Student>>(byteArr);
                _cache.SetString("getAll", json, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
                });
                return byteArr;
            }
            else // cahch available
            {
                var str = System.Text.Encoding.Default.GetString(response);
                return MyConverter.DeserializeJson<IEnumerable<Student>>(str);
            }
           // return await _studentRepository.Get();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudents(int id)
        {

            var response = _cache.Get($"getStudentById{id}");
            if (response == null) // no cach avalable
            {
                var byteArr = await _studentRepository.Get(id);
                string json = MyConverter.SerializeJson<Student>(byteArr);
                _cache.SetString($"getStudentById{id}", json, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
                });
                return byteArr;
            }
            else // cahch available
            {
                var str = System.Text.Encoding.Default.GetString(response);
                return MyConverter.DeserializeJson < Student > (str);
            }

        }

        [HttpPost]
        public async Task<ActionResult<Student>> PostBooks([FromBody] Student student)
        {
            if (isAgeNotValid(student.Age))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Sorry, age not valid");
            }
            else
            {
                var newStudent = await _studentRepository.Create(student);
                return CreatedAtAction(nameof(GetStudents), new { id = newStudent.Id }, newStudent);
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutBooks(int id, [FromBody] Student student)
        {
            if (id != student.Id || isAgeNotValid(student.Age))
            {
                return BadRequest();
            }
            await _studentRepository.Update(student);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var studentToDelete = await _studentRepository.Get(id);
            if (studentToDelete == null)
                return NotFound();
            await _studentRepository.Delete(studentToDelete.Id);
            return NoContent();
        }
    }
}
