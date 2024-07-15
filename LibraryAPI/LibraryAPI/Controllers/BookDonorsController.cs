using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookDonorsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookDonorsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookDonors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDonor>>> GetBookDonor()
        {
          if (_context.BookDonor == null)
          {
              return NotFound();
          }
            return await _context.BookDonor.ToListAsync();
        }

        // GET: api/BookDonors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDonor>> GetBookDonor(int id)
        {
          if (_context.BookDonor == null)
          {
              return NotFound();
          }
            var bookDonor = await _context.BookDonor.FindAsync(id);

            if (bookDonor == null)
            {
                return NotFound();
            }

            return bookDonor;
        }

        // PUT: api/BookDonors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookDonor(int id, BookDonor bookDonor)
        {
            if (id != bookDonor.BooksId)
            {
                return BadRequest();
            }

            _context.Entry(bookDonor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookDonorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BookDonors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookDonor>> PostBookDonor(BookDonor bookDonor)
        {
          if (_context.BookDonor == null)
          {
              return Problem("Entity set 'ApplicationContext.BookDonor'  is null.");
          }
            _context.BookDonor.Add(bookDonor);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookDonorExists(bookDonor.BooksId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookDonor", new { id = bookDonor.BooksId }, bookDonor);
        }

        // DELETE: api/BookDonors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookDonor(int id)
        {
            if (_context.BookDonor == null)
            {
                return NotFound();
            }
            var bookDonor = await _context.BookDonor.FindAsync(id);
            if (bookDonor == null)
            {
                return NotFound();
            }

            _context.BookDonor.Remove(bookDonor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookDonorExists(int id)
        {
            return (_context.BookDonor?.Any(e => e.BooksId == id)).GetValueOrDefault();
        }
    }
}
