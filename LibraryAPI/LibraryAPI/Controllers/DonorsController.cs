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
    public class DonorsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public DonorsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Donors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Donor>>> GetDonors()
        {
          if (_context.Donors == null)
          {
              return NotFound();
          }
            return await _context.Donors.ToListAsync();
        }

        // GET: api/Donors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Donor>> GetDonor(int id)
        {
          if (_context.Donors == null)
          {
              return NotFound();
          }
            var donor = await _context.Donors.FindAsync(id);

            if (donor == null)
            {
                return NotFound();
            }

            return donor;
        }

        // PUT: api/Donors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDonor(int id, Donor donor)
        {
            if (id != donor.Id)
            {
                return BadRequest();
            }

            _context.Entry(donor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonorExists(id))
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

        // POST: api/Donors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Donor>> PostDonor(Donor donor)
        {
          if (_context.Donors == null)
          {
              return Problem("Entity set 'ApplicationContext.Donors'  is null.");
          }
            _context.Donors.Add(donor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDonor", new { id = donor.Id }, donor);
        }

        // DELETE: api/Donors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonor(int id)
        {
            if (_context.Donors == null)
            {
                return NotFound();
            }
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null)
            {
                return NotFound();
            }

            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("BorrowBook/{donorId}/{bookCopyId}")]
        public async Task<IActionResult> BorrowBook(int donorId, int bookCopyId)
        {
            var donor = await _context.Donors.Include(m => m.BorrowedBooks).FirstOrDefaultAsync(m => m.Id == donorId);

            if (donor == null)
            {
                return NotFound("Donor not found");
            }


            var bookCopy = await _context.BookCopies.FindAsync(bookCopyId);
            if (bookCopy == null || !bookCopy.IsAvailable)
            {
                return NotFound("Book copy not available");
            }


            if (donor.BorrowedBooks != null && donor.BorrowedBooks.Count >= 2)
            {
                return BadRequest("You cannot borrow more than 2 books");
            }

            donor.BorrowedBooks.Add(bookCopy);
            bookCopy.IsAvailable = false;

            _context.Donors.Update(donor);
            _context.BookCopies.Update(bookCopy);

            await _context.SaveChangesAsync(); //Veri tabanı değişikliklerini kaydetmek için kullanılır.

            return Ok("Book borrowed successfully");
        }

        [HttpPost("DeliverBook/{donorId}/{bookCopyId}")]
        public async Task<IActionResult> DeliverBook(int donorId, int bookCopyId, int ratingValue)
        {

            //Applicationuser ile yap burayı
            var donor = await _context.Donors.Include(m => m.BorrowedBooks).Include(m => m.DeliveredBooks).FirstOrDefaultAsync(m => m.Id == donorId);

            if (donor == null)
            {
                return NotFound("Donor Not Found");
            }

            var bookCopy = await _context.BookCopies.FindAsync(bookCopyId);

            if (bookCopy == null)
            {
                return NotFound("Book copy not avaliable");
            }

            if (bookCopy.IsAvailable)
            {
                return BadRequest("Book copy is already available");
            }


            if (!donor.BorrowedBooks.Contains(bookCopy))
            {
                return BadRequest("Book copy was not delivered by this member");
            }

            var rating = await _context.Ratings.FirstOrDefaultAsync(d => d.BookCopy.Id == bookCopyId);

            if (rating == null)
            {
                // If rating for this book copy doesn't exist, create a new one
                rating = new Rating
                {
                    DonorId1 = donorId,
                    RatingSum = ratingValue,
                    RatingAmount = 1,
                    BookCopy = bookCopy
                };

                _context.Ratings.Add(rating);
            }
            else
            {
                // Update existing rating
                rating.RatingSum += ratingValue;
                rating.RatingAmount++;
                // Update average rating
                rating.AverageRating = (double)rating.RatingSum / rating.RatingAmount;
                _context.Ratings.Update(rating);
            }
            


            donor.DeliveredBooks.Add(bookCopy);
            donor.BorrowedBooks.Remove(bookCopy);
            bookCopy.IsAvailable = true;

            _context.Donors.Update(donor);
            _context.BookCopies.Update(bookCopy);

            await _context.SaveChangesAsync();

            return Ok("Book delivered successfully");
        }



        [HttpGet("BorrowedBookList/{donorId}")]
        public async Task<IActionResult> BorrowedBookList(int donorId)
        {
            // Üyeyi ve kiraladığı kitapları yükle
            var donor = await _context.Donors
                .Include(m => m.BorrowedBooks)
                .ThenInclude(b => b.Book)  //// Kitap bilgilerini de yükleyin
                .FirstOrDefaultAsync(m => m.Id == donorId);

            if (donor == null)
            {
                return NotFound("Donor not found");
            }

            var borrowedBooks = donor.BorrowedBooks.Select(b => new
            {
                b.Book.Id,
                b.Book.Title,
                BorrowDate = b.BorrowDate?.ToString(),
                LocationShelf = b.LocationShelf?.ToString()
            }).ToList();

            return Ok(borrowedBooks);
        }

        private bool DonorExists(int id)
        {
            return (_context.Donors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
