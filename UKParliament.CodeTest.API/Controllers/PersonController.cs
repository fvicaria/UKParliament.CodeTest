using Microsoft.AspNetCore.Mvc;

using UKParliament.CodeTest.Common.Interfaces;
using UKParliament.CodeTest.Common.Entities;

namespace UKParliament.CodeTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        // GET: api/person
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetAll()
        {
            try
            {
                var persons = await _personService.GetAllPeopleAsync();
                return Ok(persons);
            }
            catch (Exception ex)
            {
                // Typically, a catch-all is used for logging or returning a 500-level error.
                // For demonstration:
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/person/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetById(int id)
        {
            try
            {
                var person = await _personService.GetPersonByIdAsync(id);
                return Ok(person);
            }
            catch (KeyNotFoundException ex)
            {
                // Convert business/domain exception to a NotFound response
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Catch-all for other errors
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/person
        [HttpPost]
        public async Task<ActionResult<Person>> Post([FromBody] Person person)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdPerson = await _personService.AddPersonAsync(person);
                return CreatedAtAction(nameof(GetById), new { id = createdPerson.Id }, createdPerson);
            }
            catch (ArgumentException ex)
            {
                // Argument exceptions often map to HTTP 400
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/person/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Person person)
        {
            try
            {
                if (id != person.Id)
                {
                    return BadRequest("ID mismatch between route and entity.");
                }

                var success = await _personService.UpdatePersonAsync(person);
                return Ok(success);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/person/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _personService.DeletePersonAsync(id);
                return Ok(success);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }


}
