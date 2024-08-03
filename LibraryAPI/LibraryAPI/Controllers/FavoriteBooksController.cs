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
    public class FavoriteBooksController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public FavoriteBooksController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("FavoritedBookList/{memberId}")]
        public async Task<IActionResult> FavoritedBookList(string memberId)
        {
            try
            {
                // Üyeyi ve favori kitapları yükle, BookCopy ve Book bilgilerini de yükleyin
                var member = await _context.Members
                    .Include(m => m.FavoriteBooks)
                        .ThenInclude(fb => fb.BookCopy) // FavoriteBook'un BookCopy özelliğini yükle
                            .ThenInclude(bc => bc.Book) // BookCopy'un Book özelliğini yükle
                    .FirstOrDefaultAsync(m => m.Id == memberId);

                if (member == null)
                {
                    return NotFound("Member not found");
                }

                // Favori kitapları seç ve gerekli bilgileri projeksiyon yaparak al
                var favoritedBooks = member.FavoriteBooks.Select(fb => new
                {
                    BookId = fb.BookCopy.Book.Id,
                    Title = fb.BookCopy.Book.Title,
                    FavoritedDate = fb.FavoriteDate.HasValue ? fb.FavoriteDate.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") : null,
                }).ToList();


                return Ok(favoritedBooks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("FavoritedBookkList/{employeeId}")]
        public async Task<IActionResult> FavoritedBookkList(string employeeId)
        {
            try
            {
                // Üyeyi ve favori kitapları yükle, BookCopy ve Book bilgilerini de yükleyin
                var employee = await _context.Employees
                    .Include(m => m.FavoriteBooks)
                        .ThenInclude(fb => fb.BookCopy) // FavoriteBook'un BookCopy özelliğini yükle
                            .ThenInclude(bc => bc.Book) // BookCopy'un Book özelliğini yükle
                    .FirstOrDefaultAsync(m => m.Id == employeeId);

                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                // Favori kitapları seç ve gerekli bilgileri projeksiyon yaparak al
                var favoritedBooks = employee.FavoriteBooks.Select(fb => new
                {
                    BookId = fb.BookCopy.Book.Id,
                    Title = fb.BookCopy.Book.Title,
                    FavoritedDate = fb.FavoriteDate.HasValue ? fb.FavoriteDate.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") : null,
                }).ToList();


                return Ok(favoritedBooks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/FavoriteBooks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FavoriteBook>> GetFavoriteBook(int id)
        {
          if (_context.FavoriteBooks == null)
          {
              return NotFound();
          }
            var favoriteBook = await _context.FavoriteBooks.FindAsync(id);

            if (favoriteBook == null)
            {
                return NotFound();
            }

            return favoriteBook;
        }

        // PUT: api/FavoriteBooks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFavoriteBook(string id, FavoriteBook favoriteBook)
        {
            if (id != favoriteBook.Id)
            {
                return BadRequest();
            }

            _context.Entry(favoriteBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FavoriteBookExists(id))
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

        [HttpPost("FavoriteBook/{memberId}/{bookCopyId}")]
        public async Task<IActionResult> FavoriteBook(string memberId, int bookCopyId)
        {
            try
            {
                var member = await _context.Members
                    .Include(m => m.FavoriteBooks)
                    .FirstOrDefaultAsync(m => m.Id == memberId);

                if (member == null)
                {
                    return NotFound("Member not found");
                }

                // Doğru şekilde BookCopyId property'sini kullanın
                var existingFavorite = member.FavoriteBooks.FirstOrDefault(fb => fb.BookCopyId == bookCopyId);
                if (existingFavorite != null)
                {
                    return BadRequest("Book copy already added to favorites");
                }

                var bookCopy = await _context.BookCopies.FindAsync(bookCopyId);
                if (bookCopy == null)
                {
                    return NotFound("Book copy not available");
                }

                var newFavorite = new FavoriteBook
                {
                    Id = Guid.NewGuid().ToString(),
                    BookCopyId = bookCopyId,
                    MemberId = memberId,
                    FavoriteDate = DateTime.UtcNow
                };

                _context.FavoriteBooks.Add(newFavorite);
                await _context.SaveChangesAsync();

                return Ok("Book added to favorites successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("RemoveBook/{memberId}/{bookCopyId}")]
        public async Task<IActionResult> RemoveBook(string memberId, int bookCopyId)
        {
            try
            {
                var member = await _context.Members
                    .Include(m => m.FavoriteBooks)
                    .FirstOrDefaultAsync(m => m.Id == memberId);

                if (member == null)
                {
                    return NotFound("Member not found");
                }

                // Find the FavoriteBook to remove based on both MemberId and BookCopyId
                var favoriteBookToRemove = member.FavoriteBooks.FirstOrDefault(fb => fb.MemberId == memberId && fb.BookCopyId == bookCopyId);

                if (favoriteBookToRemove == null)
                {
                    return BadRequest("Book copy was not removed by this member");
                }

                _context.FavoriteBooks.Remove(favoriteBookToRemove);

                _context.Members.Update(member);
                await _context.SaveChangesAsync();

                return Ok("Book removed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost("FavoriteBookk/{employeeId}/{bookCopyId}")]
        public async Task<IActionResult> FavoriteBookk(string employeeId, int bookCopyId)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(m => m.FavoriteBooks)
                    .FirstOrDefaultAsync(m => m.Id == employeeId);

                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                // Doğru şekilde BookCopyId property'sini kullanın
                var existingFavorite = employee.FavoriteBooks.FirstOrDefault(fb => fb.BookCopyId == bookCopyId);
                if (existingFavorite != null)
                {
                    return BadRequest("Book copy already added to favorites");
                }

                var bookCopy = await _context.BookCopies.FindAsync(bookCopyId);
                if (bookCopy == null)
                {
                    return NotFound("Book copy not available");
                }

                var newFavorite = new FavoriteBook
                {
                    Id = Guid.NewGuid().ToString(),
                    BookCopyId = bookCopyId,
                    EmployeeId = employeeId,
                    FavoriteDate = DateTime.UtcNow
                };

                _context.FavoriteBooks.Add(newFavorite);
                await _context.SaveChangesAsync();

                return Ok("Book added to favorites successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("RemoveBookk/{employeeId}/{bookCopyId}")]
        public async Task<IActionResult> RemoveBookk(string employeeId, int bookCopyId)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(m => m.FavoriteBooks)
                    .FirstOrDefaultAsync(m => m.Id == employeeId);

                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                // Find the FavoriteBook to remove based on both MemberId and BookCopyId
                var favoriteBookToRemove = employee.FavoriteBooks.FirstOrDefault(fb => fb.EmployeeId == employeeId && fb.BookCopyId == bookCopyId);

                if (favoriteBookToRemove == null)
                {
                    return BadRequest("Book copy was not removed by this member");
                }

                _context.FavoriteBooks.Remove(favoriteBookToRemove);

                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();

                return Ok("Book removed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }





        private bool FavoriteBookExists(string id)
        {
            return (_context.FavoriteBooks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
