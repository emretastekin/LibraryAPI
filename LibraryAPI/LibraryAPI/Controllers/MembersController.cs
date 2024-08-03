using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public MembersController(ApplicationContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
          if (_context.Members == null)
          {
              return NotFound();
          }
            return await _context.Members.ToListAsync();
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(string id)
        {
          if (_context.Members == null)
          {
              return NotFound();
          }
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(string id, Member member,string? currentPassword=null)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(id).Result;

            if (id != member.Id)
            {
                return BadRequest();
            }

            applicationUser.Address = member.ApplicationUser!.Address;
            applicationUser.BirthDate = member.ApplicationUser!.BirthDate;
            applicationUser.Email = member.ApplicationUser!.Email;

            _userManager.UpdateAsync(applicationUser).Wait();

            if (currentPassword != null)
            {
                _userManager.ChangePasswordAsync(applicationUser, currentPassword, applicationUser.Password).Wait();
            }
            member.ApplicationUser = null;

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
          if (_context.Members == null)
          {
              return Problem("Entity set 'ApplicationContext.Members'  is null.");
          }
            _userManager.CreateAsync(member.ApplicationUser!,member.ApplicationUser!.Password).Wait();
            _userManager.AddToRoleAsync(member.ApplicationUser!, "Member").Wait();
            


            member.Id = member.ApplicationUser!.Id;
            member.ApplicationUser = null;
            _context.Members.Add(member);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MemberExists(member.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        [HttpPost("upload-cover-image/{memberId}")]
        public async Task<IActionResult> UploadCoverImage(string memberId, IFormFile coverImage)
        {
            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Kitabı bul
            var member = await _context.Members.FindAsync(memberId);
            if (member == null)
            {
                return NotFound("Member not found.");
            }

            // Dosya yolunu belirleyin (örneğin: wwwroot/images/{fileName})
            var fileName = coverImage.FileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            // Dosyayı kaydedin
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await coverImage.CopyToAsync(stream);
            }

            // Kitap nesnesinin CoverImageUrl özelliğini güncelleyin
            member.CoverImageUrl = $"/images/{fileName}";

            // Kitap nesnesini güncelleyin
            _context.Entry(member).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Dosyayı okuyup yanıt olarak döndürün
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/jpeg");

        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(string id)
        {
            if (_context.Members == null)
            {
                return NotFound();
            }
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

                

        [HttpPost("BorrowBook/{memberId}/{bookCopyId}")]
        public async Task<IActionResult> BorrowBook(string memberId, int bookCopyId)
        {
            var member = await _context.Members.Include(m => m.BorrowedBooks).FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
            {
                return NotFound("Member not found");
            }


            var bookCopy = await _context.BookCopies.FindAsync(bookCopyId);
            if(bookCopy==null || !bookCopy.IsAvailable)
            {
                return NotFound("Book copy not available");
            }


            if (member.BorrowedBooks != null && member.BorrowedBooks.Count >= 2  )
            {
                return BadRequest("You cannot borrow more than 2 books");
            }

            member.BorrowedBooks.Add(bookCopy);
            bookCopy.IsAvailable = false;
            bookCopy.BorrowingMemberId = memberId;

            _context.Members.Update(member);
            _context.BookCopies.Update(bookCopy);

            await _context.SaveChangesAsync(); //Veri tabanı değişikliklerini kaydetmek için kullanılır.

            return Ok("Book borrowed successfully");
        }

        [HttpPost("DeliverBook/{memberId}/{bookCopyId}")]
        public async Task<IActionResult> DeliverBook(string memberId, int bookCopyId, int ratingValue)
        {

            //Applicationuser ile yap burayı
            var member = await _context.Members.Include(m => m.BorrowedBooks).Include(m => m.DeliveredBooks).FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
            {
                return NotFound("Member Not Found");
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


            if (!member.BorrowedBooks.Contains(bookCopy))
            {
                return BadRequest("Book copy was not delivered by this member");
            }

            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.BookCopy.Id == bookCopyId);


            if (rating == null)
            {
                // If rating for this book copy doesn't exist, create a new one
                rating = new Rating
                {
                    MemberId=memberId,
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




            member.DeliveredBooks.Add(bookCopy);
            member.BorrowedBooks.Remove(bookCopy);
            bookCopy.IsAvailable = true;
            bookCopy.DeliveringMemberId = memberId;

            _context.Members.Update(member);
            _context.BookCopies.Update(bookCopy);

            await _context.SaveChangesAsync();

            return Ok("Book delivered successfully");
           


        }

        [HttpGet("BorrowedBookList/{memberId}")]
        public async Task<IActionResult> BorrowedBookList(string memberId)
        {
            // Üyeyi ve kiraladığı kitapları yükle
            var member = await _context.Members
                .Include(m => m.BorrowedBooks)
                .ThenInclude(b => b.Book)  //// Kitap bilgilerini de yükleyin
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
            {
                return NotFound("Member not found");
            }

            var borrowedBooks = member.BorrowedBooks.Select(b => new
            {
                b.Book.Id,
                b.Book.Title,
                BorrowDate = b.BorrowDate?.ToString(),
                LocationShelf=b.LocationShelf?.ToString()
            }).ToList();

            return Ok(borrowedBooks);
        }





        private bool MemberExists(string id)
        {
            return (_context.Members?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
    }
}
