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
    public class BookLibrariesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookLibrariesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookLibraries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookLibrary>>> GetBookLibrary()
        {
          if (_context.BookLibrary == null)
          {
              return NotFound();
          }
            return await _context.BookLibrary.ToListAsync();
        }

        // GET: api/BookLibraries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookLibrary>> GetBookLibrary(int id)
        {
          if (_context.BookLibrary == null)
          {
              return NotFound();
          }
            var bookLibrary = await _context.BookLibrary.FindAsync(id);

            if (bookLibrary == null)
            {
                return NotFound();
            }

            return bookLibrary;
        }

        // PUT: api/BookLibraries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookLibrary(int id, BookLibrary bookLibrary)
        {
            if (id != bookLibrary.BooksId)
            {
                return BadRequest();
            }

            _context.Entry(bookLibrary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookLibraryExists(id))
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

        // POST: api/BookLibraries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookLibrary>> PostBookLibrary(BookLibrary bookLibrary)
        {
          if (_context.BookLibrary == null)
          {
              return Problem("Entity set 'ApplicationContext.BookLibrary'  is null.");
          }
            _context.BookLibrary.Add(bookLibrary);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookLibraryExists(bookLibrary.BooksId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookLibrary", new { id = bookLibrary.BooksId }, bookLibrary);
        }

        // DELETE: api/BookLibraries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookLibrary(int id)
        {
            if (_context.BookLibrary == null)
            {
                return NotFound();
            }
            var bookLibrary = await _context.BookLibrary.FindAsync(id);
            if (bookLibrary == null)
            {
                return NotFound();
            }

            _context.BookLibrary.Remove(bookLibrary);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookLibraryExists(int id)
        {
            return (_context.BookLibrary?.Any(e => e.BooksId == id)).GetValueOrDefault();
        }
    }
}
