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
using QRCoder;
using SkiaSharp;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin,Employee,Member")]
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

        [Authorize(Roles = "Admin,Employee,Member")]
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

        [Authorize(Roles = "Admin,Employee,Member")]
        [HttpPost("upload-cover-image/{memberId}")]
        public async Task<IActionResult> UploadCoverImage(string memberId, IFormFile coverImage)
        {
            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Kişiyi bul
            var member = await _context.Members.FindAsync(memberId);
            if (member == null)
            {
                return NotFound("Member not found.");
            }

            // Dosya yolunu belirleyin (örneğin: wwwroot/images/{fileName})
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            // Klasörün var olup olmadığını kontrol edin ve gerekiyorsa oluşturun
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = coverImage.FileName;
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Dosyayı kaydedin
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await coverImage.CopyToAsync(stream);
            }

            // Kişinin CoverImageUrl özelliğini güncelleyin
            member.CoverImageUrl = $"/images/{fileName}";

            // Kişi nesnesini güncelleyin
            _context.Entry(member).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Dosyayı okuyup yanıt olarak döndürün
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/jpeg");
        }

        [Authorize(Roles = "Admin,Employee,Member")]
        [HttpDelete("remove-cover-image/{memberId}")]
        public async Task<IActionResult> RemoveCoverImage(string memberId)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member == null)
            {
                return NotFound("Member not found.");
            }

            // Eski kapak resminin dosya yolunu belirleyin
            var oldFileName = Path.GetFileName(member.CoverImageUrl);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", oldFileName);

            // Dosya varsa, dosyayı kaldırın
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Kapak resmini kaldırın
            member.CoverImageUrl = null;

            // Üye nesnesini güncelleyin
            _context.Entry(member).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /*
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
        */

        [Authorize(Roles = "Admin")]
        [HttpPut("Deactivate/{memberId}")]
        public async Task<IActionResult> DeactivateMember(string memberId)
        {
            var member = await _context.Members
                .Include(m => m.ApplicationUser) // ApplicationUser ile ilişkiyi dahil ediyoruz
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
            {
                return NotFound();
            }

            member.IsActive = false;

            if (member.ApplicationUser != null)
            {
                member.ApplicationUser.IsActive = false;
            }

            _context.Entry(member).State = EntityState.Modified;

            if (member.ApplicationUser != null)
            {
                _context.Entry(member.ApplicationUser).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("Activate/{memberId}")]
        public async Task<IActionResult> ActivateMember(string memberId)
        {
            var member = await _context.Members
                .Include(m => m.ApplicationUser) // ApplicationUser ile ilişkiyi dahil ediyoruz
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
            {
                return NotFound();
            }

            member.IsActive = true;

            if (member.ApplicationUser != null)
            {
                member.ApplicationUser.IsActive = true;
            }

            _context.Entry(member).State = EntityState.Modified;

            if (member.ApplicationUser != null)
            {
                _context.Entry(member.ApplicationUser).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [Authorize(Roles = "Admin,Member")]
        [HttpPost("BorrowBook/{memberId}/{bookCopyId}")]
        public async Task<IActionResult> BorrowBook(string memberId, int bookCopyId)
        {
            // Üyenin var olup olmadığını kontrol et ve BorrowedBooks ile birlikte getir
            var member = await _context.Members.Include(m => m.BorrowedBooks).FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null || !member.IsActive)
            {
                return NotFound("Member not found or inactive");
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
            if (member.BorrowedBooks != null && member.BorrowedBooks.Count >= 2)
            {
                return BadRequest("You cannot borrow more than 2 books");
            }

            // Üyenin aynı kitap kopyasını daha önce ödünç alıp almadığını kontrol et
            if (member.BorrowedBooks != null && member.BorrowedBooks.Any(b => b.Id == bookCopyId))
            {
                return BadRequest("You have already borrowed this book copy");
            }

            // Kitap ödünç alma işlemi
            member.BorrowedBooks.Add(bookCopy);
            bookCopy.BorrowingMemberId = memberId;
            bookCopy.BorrowDate = DateTime.Now; // Burada BorrowDate özelliğini güncelliyoruz

            // Stok sayısını güncelle
            bookCopy.StockNumber--;

            // Stok sayısı 0 olduğunda IsAvailable özelliğini false yap
            if (bookCopy.StockNumber == 0)
            {
                bookCopy.IsAvailable = false;
            }

            _context.Members.Update(member);
            _context.BookCopies.Update(bookCopy);

            // Veri tabanı değişikliklerini kaydet
            await _context.SaveChangesAsync();

            return Ok("Book borrowed successfully");
        }


        [Authorize(Roles = "Admin,Member")]
        [HttpPost("DeliverBook/{memberId}/{bookCopyId}")]
        public async Task<IActionResult> DeliverBook(string memberId, int bookCopyId, int ratingValue)
        {
            // Üyenin var olup olmadığını kontrol et ve BorrowedBooks ve DeliveredBooks ile birlikte getir
            var member = await _context.Members.Include(m => m.BorrowedBooks).Include(m => m.DeliveredBooks).FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null || !member.IsActive)
            {
                return NotFound("Member not found or inactive");
            }

            // Kitap kopyasının var olup olmadığını kontrol et
            var bookCopy = await _context.BookCopies.FindAsync(bookCopyId);

            if (bookCopy == null)
            {
                return NotFound("Book copy not available");
            }

            /*
            // Kitap kopyasının zaten müsait olup olmadığını kontrol et
            if (bookCopy.IsAvailable)
            {
                return BadRequest("Book copy is already available");
            }
            */

            // Üyenin bu kitap kopyasını ödünç alıp almadığını kontrol et
            if (!member.BorrowedBooks.Contains(bookCopy))
            {
                return BadRequest("Book copy was not borrowed by this member");
            }

            // Kitap kopyası için rating'i kontrol et veya yeni bir rating oluştur
            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.BookCopy.Id == bookCopyId);

            if (rating == null)
            {
                // Eğer bu kitap kopyası için rating yoksa, yeni bir rating oluştur
                rating = new Rating
                {
                    MemberId = memberId,
                    RatingSum = ratingValue,
                    RatingAmount = 1,
                    BookCopy = bookCopy
                };

                _context.Ratings.Add(rating);
            }
            else
            {
                // Mevcut rating'i güncelle
                rating.RatingSum += ratingValue;
                rating.RatingAmount++;
                // Ortalama rating'i güncelle
                rating.AverageRating = (double)rating.RatingSum / rating.RatingAmount;
                _context.Ratings.Update(rating);
            }

            // Üyenin borrowed ve delivered books listelerini güncelle
            member.DeliveredBooks.Add(bookCopy);
            member.BorrowedBooks.Remove(bookCopy);

            // Kitap kopyasının durumunu güncelle
            bookCopy.IsAvailable = true;
            bookCopy.DeliveringMemberId = memberId;
            bookCopy.DeliveredDate = DateTime.Now;
            bookCopy.StockNumber++;  // Stok sayısını artır

            _context.Members.Update(member);
            _context.BookCopies.Update(bookCopy);

            // Veri tabanı değişikliklerini kaydet
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

            if (member == null )
            {
                return NotFound("Member not found ");
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

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("QRCodeGenerator/{memberId}")]
        public async Task<IActionResult> Generate(string memberId)
        {
            // Member verisini veritabanından al
            var member = await _context.Members
                .Include(e => e.ApplicationUser) // ApplicationUser ilişkisini dahil edin
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
            {
                return NotFound("Member not found.");
            }

            // QR kodu içeriğini oluşturun
            var qrCodeContent = $"Üye ID: {member.Id}\n" +
                                $"Üye Adı: {member.ApplicationUser?.Name}\n" +
                                $"Adres: {member.ApplicationUser?.Address}\n" +
                                $"Üye Telefon: {member.ApplicationUser?.PhoneNumber}\n" +
                                $"Aktif: {member.IsActive}\n";

            byte[] qrCodeBytes;

            // QR kodunu oluştur
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new BitmapByteQRCode(qrCodeData);

                // QR kodunu byte[] olarak al
                var qrCodeImage = qrCode.GetGraphic(5);

                // byte[]'i SKBitmap'e dönüştür
                using (var skBitmap = SKBitmap.Decode(qrCodeImage))
                {
                    using (var stream = new MemoryStream())
                    {
                        skBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
                        qrCodeBytes = stream.ToArray();
                    }
                }
            }

            // QR kodunu PNG olarak döndür
            return File(qrCodeBytes, "image/png");
        }




        private bool MemberExists(string id)
        {
            return (_context.Members?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
    }
}
