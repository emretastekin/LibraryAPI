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
    public class BookRoomsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookRoomsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookRooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookRoom>>> GetBookRoom()
        {
          if (_context.BookRoom == null)
          {
              return NotFound();
          }
            return await _context.BookRoom.ToListAsync();
        }

        // GET: api/BookRooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookRoom>> GetBookRoom(int id)
        {
          if (_context.BookRoom == null)
          {
              return NotFound();
          }
            var bookRoom = await _context.BookRoom.FindAsync(id);

            if (bookRoom == null)
            {
                return NotFound();
            }

            return bookRoom;
        }

        // PUT: api/BookRooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookRoom(int id, BookRoom bookRoom)
        {
            if (id != bookRoom.BooksId)
            {
                return BadRequest();
            }

            _context.Entry(bookRoom).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookRoomExists(id))
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

        // POST: api/BookRooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookRoom>> PostBookRoom(BookRoom bookRoom)
        {
          if (_context.BookRoom == null)
          {
              return Problem("Entity set 'ApplicationContext.BookRoom'  is null.");
          }
            _context.BookRoom.Add(bookRoom);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookRoomExists(bookRoom.BooksId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookRoom", new { id = bookRoom.BooksId }, bookRoom);
        }

        // DELETE: api/BookRooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookRoom(int id)
        {
            if (_context.BookRoom == null)
            {
                return NotFound();
            }
            var bookRoom = await _context.BookRoom.FindAsync(id);
            if (bookRoom == null)
            {
                return NotFound();
            }

            _context.BookRoom.Remove(bookRoom);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookRoomExists(int id)
        {
            return (_context.BookRoom?.Any(e => e.BooksId == id)).GetValueOrDefault();
        }
    }
}
