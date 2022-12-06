using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trackingAPI.Data.Contexts;
using trackingAPI.Models;
using trackingAPI.MiddleWare.Decorators;

namespace trackingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly IssueDbContext _context;

        public IssueController(IssueDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
        public async Task<IEnumerable<Issue>> Get()
        {
            return await _context.Issues.ToListAsync();
        }

        [HttpGet("{id}")] //Id param in url bound to id param in method
        [ProducesResponseType(typeof(Issue), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
        public async Task<IActionResult> GetById(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            return issue == null ? NotFound() : Ok(issue);
        }

        [HttpPost] //issue found in the body of the request
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
        public async Task<IActionResult> Create(Issue issue)
        {
            try
            {
                await _context.AddAsync(issue);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _context.Entry(issue).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetById), new { Id = issue.Id }, issue);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
        public async Task<IActionResult> Update(int id, Issue issue)
        {
            if (id != issue.Id) return BadRequest();

            try
            {
                _context.Issues.Entry(issue).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _context.Issues.Entry(issue).State = EntityState.Unchanged;
                return BadRequest();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
        public async Task<IActionResult> Delete(int id)
        {
            var issueToDelete = await _context.Issues.FindAsync(id);
            if (issueToDelete == null) return NotFound();

            _context.Issues.Remove(issueToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
