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
    public class BookTranslatorsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookTranslatorsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookTranslators
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookTranslator>>> GetBookTranslator()
        {
          if (_context.BookTranslator == null)
          {
              return NotFound();
          }
            return await _context.BookTranslator.ToListAsync();
        }

        // GET: api/BookTranslators/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookTranslator>> GetBookTranslator(int id)
        {
          if (_context.BookTranslator == null)
          {
              return NotFound();
          }
            var bookTranslator = await _context.BookTranslator.FindAsync(id);

            if (bookTranslator == null)
            {
                return NotFound();
            }

            return bookTranslator;
        }

        // PUT: api/BookTranslators/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookTranslator(int id, BookTranslator bookTranslator)
        {
            if (id != bookTranslator.BooksId)
            {
                return BadRequest();
            }

            _context.Entry(bookTranslator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookTranslatorExists(id))
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

        // POST: api/BookTranslators
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookTranslator>> PostBookTranslator(BookTranslator bookTranslator)
        {
          if (_context.BookTranslator == null)
          {
              return Problem("Entity set 'ApplicationContext.BookTranslator'  is null.");
          }
            _context.BookTranslator.Add(bookTranslator);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookTranslatorExists(bookTranslator.BooksId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookTranslator", new { id = bookTranslator.BooksId }, bookTranslator);
        }

        // DELETE: api/BookTranslators/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookTranslator(int id)
        {
            if (_context.BookTranslator == null)
            {
                return NotFound();
            }
            var bookTranslator = await _context.BookTranslator.FindAsync(id);
            if (bookTranslator == null)
            {
                return NotFound();
            }

            _context.BookTranslator.Remove(bookTranslator);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookTranslatorExists(int id)
        {
            return (_context.BookTranslator?.Any(e => e.BooksId == id)).GetValueOrDefault();
        }
    }
}
