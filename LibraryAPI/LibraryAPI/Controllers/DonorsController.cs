using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin,Employee,Member")]
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
        [Authorize(Roles = "Admin,Employee,Member")]
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
        [Authorize(Roles = "Admin,Employee,Member")]
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

        [Authorize(Roles = "Admin,Member,Employee")]
        [HttpPost("BorrowBook/{donorId}/{bookCopyId}")]
        public async Task<IActionResult> BorrowBook(int donorId, int bookCopyId)
        {
            // Üyenin var olup olmadığını kontrol et ve BorrowedBooks ile birlikte getir
            var donor = await _context.Donors.Include(m => m.BorrowedBooks).FirstOrDefaultAsync(m => m.Id == donorId);

            if (donor == null)
            {
                return NotFound("Donor not found");
            }

            // Kitap kopyasının var olup olmadığını ve müsait olup olmadığını kontrol et
            var bookCopy = await _context.BookCopies.FindAsync(bookCopyId);
            if (bookCopy == null)
            {
                return NotFound("Book copy not available");
            }

            // Kitap kopyasının stok sayısını kontrol et
            if (bookCopy.StockNumber <= 0)
            {
                return BadRequest("No copies of the book are currently available");
            }

            // Üyenin halihazırda ödünç aldığı kitap sayısını kontrol et
            if (donor.BorrowedBooks != null && donor.BorrowedBooks.Count >= 2)
            {
                return BadRequest("You cannot borrow more than 2 books");
            }

            // Üyenin aynı kitap kopyasını daha önce ödünç alıp almadığını kontrol et
            if (donor.BorrowedBooks != null && donor.BorrowedBooks.Any(b => b.Id == bookCopyId))
            {
                return BadRequest("You have already borrowed this book copy");
            }

            // Kitap ödünç alma işlemi
            donor.BorrowedBooks.Add(bookCopy);
            bookCopy.BorrowDate = DateTime.Now; // Burada BorrowDate özelliğini güncelliyoruz

            // Stok sayısını güncelle
            bookCopy.StockNumber--;

            // Stok sayısı 0 olduğunda IsAvailable özelliğini false yap
            if (bookCopy.StockNumber == 0)
            {
                bookCopy.IsAvailable = false;
            }

            _context.Donors.Update(donor);
            _context.BookCopies.Update(bookCopy);

            // Veri tabanı değişikliklerini kaydet
            await _context.SaveChangesAsync();

            return Ok("Book borrowed successfully");
        }

        [Authorize(Roles = "Admin,Employee,Member")]
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

            bookCopy.DeliveredDate = DateTime.Now;
            bookCopy.StockNumber++;  // Stok sayısını artır

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
