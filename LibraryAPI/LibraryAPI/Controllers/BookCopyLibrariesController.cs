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
    public class BookCopyLibrariesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookCopyLibrariesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookCopyLibraries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookCopyLibrary>>> GetBookCopyLibraries()
        {
          if (_context.BookCopyLibraries == null)
          {
              return NotFound();
          }
            return await _context.BookCopyLibraries.ToListAsync();
        }

        // GET: api/BookCopyLibraries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookCopyLibrary>> GetBookCopyLibrary(int id)
        {
          if (_context.BookCopyLibraries == null)
          {
              return NotFound();
          }
            var bookCopyLibrary = await _context.BookCopyLibraries.FindAsync(id);

            if (bookCopyLibrary == null)
            {
                return NotFound();
            }

            return bookCopyLibrary;
        }

        // PUT: api/BookCopyLibraries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookCopyLibrary(int id, BookCopyLibrary bookCopyLibrary)
        {
            if (id != bookCopyLibrary.BookCopiesId)
            {
                return BadRequest();
            }

            _context.Entry(bookCopyLibrary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookCopyLibraryExists(id))
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

        // POST: api/BookCopyLibraries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookCopyLibrary>> PostBookCopyLibrary(BookCopyLibrary bookCopyLibrary)
        {
          if (_context.BookCopyLibraries == null)
          {
              return Problem("Entity set 'ApplicationContext.BookCopyLibraries'  is null.");
          }
            _context.BookCopyLibraries.Add(bookCopyLibrary);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookCopyLibraryExists(bookCopyLibrary.BookCopiesId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookCopyLibrary", new { id = bookCopyLibrary.BookCopiesId }, bookCopyLibrary);
        }

        // DELETE: api/BookCopyLibraries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookCopyLibrary(int id)
        {
            if (_context.BookCopyLibraries == null)
            {
                return NotFound();
            }
            var bookCopyLibrary = await _context.BookCopyLibraries.FindAsync(id);
            if (bookCopyLibrary == null)
            {
                return NotFound();
            }

            _context.BookCopyLibraries.Remove(bookCopyLibrary);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookCopyLibraryExists(int id)
        {
            return (_context.BookCopyLibraries?.Any(e => e.BookCopiesId == id)).GetValueOrDefault();
        }
    }
}
