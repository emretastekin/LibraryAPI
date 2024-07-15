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
    public class BookCopyRoomsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookCopyRoomsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookCopyRooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookCopyRoom>>> GetBookCopyRoom()
        {
          if (_context.BookCopyRoom == null)
          {
              return NotFound();
          }
            return await _context.BookCopyRoom.ToListAsync();
        }

        // GET: api/BookCopyRooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookCopyRoom>> GetBookCopyRoom(int id)
        {
          if (_context.BookCopyRoom == null)
          {
              return NotFound();
          }
            var bookCopyRoom = await _context.BookCopyRoom.FindAsync(id);

            if (bookCopyRoom == null)
            {
                return NotFound();
            }

            return bookCopyRoom;
        }

        // PUT: api/BookCopyRooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookCopyRoom(int id, BookCopyRoom bookCopyRoom)
        {
            if (id != bookCopyRoom.BookCopiesId)
            {
                return BadRequest();
            }

            _context.Entry(bookCopyRoom).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookCopyRoomExists(id))
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

        // POST: api/BookCopyRooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookCopyRoom>> PostBookCopyRoom(BookCopyRoom bookCopyRoom)
        {
          if (_context.BookCopyRoom == null)
          {
              return Problem("Entity set 'ApplicationContext.BookCopyRoom'  is null.");
          }
            _context.BookCopyRoom.Add(bookCopyRoom);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookCopyRoomExists(bookCopyRoom.BookCopiesId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookCopyRoom", new { id = bookCopyRoom.BookCopiesId }, bookCopyRoom);
        }

        // DELETE: api/BookCopyRooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookCopyRoom(int id)
        {
            if (_context.BookCopyRoom == null)
            {
                return NotFound();
            }
            var bookCopyRoom = await _context.BookCopyRoom.FindAsync(id);
            if (bookCopyRoom == null)
            {
                return NotFound();
            }

            _context.BookCopyRoom.Remove(bookCopyRoom);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookCopyRoomExists(int id)
        {
            return (_context.BookCopyRoom?.Any(e => e.BookCopiesId == id)).GetValueOrDefault();
        }
    }
}
