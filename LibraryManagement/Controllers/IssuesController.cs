using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly LibraryContext _context;

        public IssuesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Issues
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Issue>>> GetIssues()
        {
            if(_context.Issues == null)
            {
                return NotFound();
            }
            return await _context.Issues.ToListAsync();
        }

        // GET: api/Issues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Issue>> GetIssue(int id)
        {
            if(_context.Issues == null)
            {
                return NotFound();
            }
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }
            return issue;
        }

        //GET: api/UserIssues/5
        [HttpGet("UserIssues/{id}")]
        public async Task<ActionResult<IEnumerable<Issue>>> UserIssues(int id)
        {
            if(_context.Issues == null)
            {
                return NotFound();
            }
            return await _context.Issues.Where(e => e.UserId == id && e.ReturnDate == null).ToListAsync();
        }

        // PUT: api/Issues/Reissue/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Reissue/{id}")]
        public async Task<IActionResult> Reissue(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }
            if (issue.ReturnDate != null)
            {
                return BadRequest();
            }
            issue.DueDate = issue.DueDate.Value.AddDays(21);
            _context.Entry(issue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // PUT: api/Issues/Return/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Return/{id}")]
        public async Task<IActionResult> Return(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }
            if (issue.ReturnDate != null)
            {
                return BadRequest();
            }
            var bookId = issue.BookId;
            var book = await _context.Books.FindAsync(bookId);
            book.Copies = book.Copies + 1;
            _context.Entry(book).State = EntityState.Modified;
            issue.ReturnDate = DateTime.Now;
            _context.Entry(issue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/Issues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Issue>> PostIssue(Issue issue)
        {
            var user = await _context.Users.FindAsync(issue.UserId);
            var book = await _context.Books.FindAsync(issue.BookId);
            if (user == null || book == null)
            {
                return NotFound();
            }
            if (book.Copies == 0)
            {
                return BadRequest();
            }
            var count = await _context.Issues.CountAsync(i => i.UserId == issue.UserId && i.ReturnDate == null);
            if (count >= 5)
            {
                return BadRequest();
            }
            book.Copies = book.Copies - 1;
            issue.IssueDate = DateTime.Now;
            issue.DueDate = DateTime.Now.AddMonths(1);

            _context.Issues.Add(issue);
            _context.Entry(book).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIssue), new { id = issue.Id }, issue);
        }

        // DELETE: api/Issues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssue(int id)
        {
            if(_context.Issues == null)
            {
                return NotFound();
            }
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }
            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool IssueExists(int id)
        {
            return _context.Issues.Any(e => e.Id == id);
        }
    }
}
