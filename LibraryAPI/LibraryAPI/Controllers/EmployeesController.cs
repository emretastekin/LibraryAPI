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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using System.Diagnostics.Metrics;
using QRCoder;
using SkiaSharp;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public EmployeesController(ApplicationContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(string id)
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee, string? currentPassword = null)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(id).Result;

            if (id != employee.Id)
            {
                return BadRequest();
            }

            //Normalde buralar if li kontrol şeklinde olmalıdır.

            applicationUser.Address = employee.ApplicationUser!.Address;
            applicationUser.BirthDate = employee.ApplicationUser!.BirthDate;
            applicationUser.Email = employee.ApplicationUser!.Email;
            //...Hepsi yapılmalıdır.

            _userManager.UpdateAsync(applicationUser).Wait(); //Frontedten alınan bilgiler ile güncelledik.

            if (currentPassword != null)
            {
                _userManager.ChangePasswordAsync(applicationUser, currentPassword, applicationUser.Password).Wait();
            }
            employee.ApplicationUser = null;


            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee, string? category = null)
        {
            Claim claim;
            if (_context.Employees == null)
            {
                return Problem("Entity set 'ApplicationContext.Employees'  is null.");
            }
            _userManager.CreateAsync(employee.ApplicationUser!, employee.ApplicationUser!.Password).Wait();
            _userManager.AddToRoleAsync(employee.ApplicationUser!, "Worker").Wait();


            if (category != null)
            {
                claim = new Claim("Category", category);
                _userManager.AddClaimAsync(employee.ApplicationUser, claim).Wait();
            }
            /*
            foreach(string category in categories)
            {
                claim = new Claim("Category",category);
                _userManager.AddClaimsAsync(employee.ApplicationUser,claim).Wait();
            }
            */
            employee.Id = employee.ApplicationUser!.Id;
            employee.ApplicationUser = null;
            _context.Employees.Add(employee);  //Employeenin içine sadece employee'nin özelliklerini yazar.
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmployeeExists(employee.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("upload-cover-image/{employeeId}")]
        public async Task<IActionResult> UploadCoverImage(string employeeId, IFormFile coverImage)
        {
            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Kişiyi bul
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
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
            employee.CoverImageUrl = $"/images/{fileName}";

            // Kişi nesnesini güncelleyin
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Dosyayı okuyup yanıt olarak döndürün
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/jpeg");
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete("remove-cover-image/{employeeId}")]
        public async Task<IActionResult> RemoveCoverImage(string employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            // Eski kapak resminin dosya yolunu belirleyin
            var oldFileName = Path.GetFileName(employee.CoverImageUrl);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", oldFileName);

            // Dosya varsa, dosyayı kaldırın
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Kapak resmini kaldırın
            employee.CoverImageUrl = null;

            // Üye nesnesini güncelleyin
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Deactivate/{employeeId}")]
        public async Task<IActionResult> DeactivateEmployee(string employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            employee.IsActive = false;
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Activate/{employeeId}")]
        public async Task<IActionResult> ActivateEmployee(string employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            employee.IsActive = true;
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /*
        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */


        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("BorrowBook/{employeeId}/{bookCopyId}")]
        public async Task<IActionResult> BorrowBook(string employeeId, int bookCopyId)
        {
            // Üyenin var olup olmadığını kontrol et ve BorrowedBooks ile birlikte getir
            var employee = await _context.Employees.Include(m => m.BorrowedBooks).FirstOrDefaultAsync(m => m.Id == employeeId);


            if (employee == null || !employee.IsActive)
            {
                return NotFound("Employee not found or inactive");
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
            if (employee.BorrowedBooks != null && employee.BorrowedBooks.Count >= 2)
            {
                return BadRequest("You cannot borrow more than 2 books");
            }

            // Üyenin aynı kitap kopyasını daha önce ödünç alıp almadığını kontrol et
            if (employee.BorrowedBooks != null && employee.BorrowedBooks.Any(b => b.Id == bookCopyId))
            {
                return BadRequest("You have already borrowed this book copy");
            }

            // Kitap ödünç alma işlemi
            employee.BorrowedBooks.Add(bookCopy);
            bookCopy.BorrowingEmployeeId = employeeId;
            bookCopy.BorrowDate = DateTime.Now;

            // Stok sayısını güncelle
            bookCopy.StockNumber--;

            // Stok sayısı 0 olduğunda IsAvailable özelliğini false yap
            if (bookCopy.StockNumber == 0)
            {
                bookCopy.IsAvailable = false;
            }

            _context.Employees.Update(employee);
            _context.BookCopies.Update(bookCopy);

            // Veri tabanı değişikliklerini kaydet
            await _context.SaveChangesAsync();

            return Ok("Book borrowed successfully");
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("DeliverBook/{employeeId}/{bookCopyId}")]
        public async Task<IActionResult> DeliverBook(string employeeId, int bookCopyId, int ratingValue)
        {
            // Üyenin var olup olmadığını kontrol et ve BorrowedBooks ve DeliveredBooks ile birlikte getir
            var employee = await _context.Employees.Include(m => m.BorrowedBooks).Include(m => m.DeliveredBooks).FirstOrDefaultAsync(m => m.Id == employeeId);

            if (employee == null || !employee.IsActive)
            {
                return NotFound("Employee not found or inactive");
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
            if (!employee.BorrowedBooks.Contains(bookCopy))
            {
                return BadRequest("Book copy was not borrowed by this employee");
            }

            // Kitap kopyası için rating'i kontrol et veya yeni bir rating oluştur
            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.BookCopy.Id == bookCopyId);

            if (rating == null)
            {
                // Eğer bu kitap kopyası için rating yoksa, yeni bir rating oluştur
                rating = new Rating
                {
                    EmployeeId = employeeId,
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
            employee.DeliveredBooks.Add(bookCopy);
            employee.BorrowedBooks.Remove(bookCopy);

            // Kitap kopyasının durumunu güncelle
            bookCopy.IsAvailable = true;
            bookCopy.DeliveringEmployeeId = employeeId;
            bookCopy.BorrowDate = DateTime.Now;
            bookCopy.StockNumber++;  // Stok sayısını artır

            _context.Employees.Update(employee);
            _context.BookCopies.Update(bookCopy);

            // Veri tabanı değişikliklerini kaydet
            await _context.SaveChangesAsync();

            return Ok("Book delivered successfully");
        }

        [HttpGet("BorrowedBookList/{employeeId}")]
        public async Task<IActionResult> BorrowedBookList(string employeeId)
        {
            // Üyeyi ve kiraladığı kitapları yükle
            var employee = await _context.Employees
                .Include(m => m.BorrowedBooks)
                .ThenInclude(b => b.Book)  //// Kitap bilgilerini de yükleyin
                .FirstOrDefaultAsync(m => m.Id == employeeId);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var borrowedBooks = employee.BorrowedBooks.Select(b => new
            {
                b.Book.Id,
                b.Book.Title,
                BorrowDate = b.BorrowDate?.ToString(),
                LocationShelf = b.LocationShelf?.ToString()
            }).ToList();

            return Ok(borrowedBooks);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("QRCodeGenerator/{employeeId}")]
        public async Task<IActionResult> Generate(string employeeId)
        {
            // Member verisini veritabanından al
            var employee = await _context.Employees
                .Include(e => e.ApplicationUser) // ApplicationUser ilişkisini dahil edin
                .FirstOrDefaultAsync(m => m.Id == employeeId);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            // QR kodu içeriğini oluşturun
            var qrCodeContent = $"Çalışan ID: {employee.Id}\n" +
                                $"Çalışan Adı: {employee.ApplicationUser?.Name}\n" +
                                $"Adres: {employee.ApplicationUser?.Address}\n" +
                                $"Telefon: {employee.ApplicationUser?.PhoneNumber}\n" +
                                $"Aktif: {employee.IsActive}\n";

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


        private bool EmployeeExists(string id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
