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

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public EmployeesController(ApplicationContext context,UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser>signInManager)
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee,string? currentPassword=null)
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

            if (currentPassword!= null)
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
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee,string? category=null)
        {
            Claim claim;
          if (_context.Employees == null)
          {
              return Problem("Entity set 'ApplicationContext.Employees'  is null.");
          }
            _userManager.CreateAsync(employee.ApplicationUser!,employee.ApplicationUser!.Password).Wait();
            _userManager.AddToRoleAsync(employee.ApplicationUser!, "Worker").Wait();
            

            if (category != null)
            {
                claim = new Claim("Category", category);
                _userManager.AddClaimAsync(employee.ApplicationUser,claim).Wait();
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

        [HttpPost("upload-cover-image/{employeeId}")]
        public async Task<IActionResult> UploadCoverImage(string employeeId, IFormFile coverImage)
        {
            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Kitabı bul
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
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
            employee.CoverImageUrl = $"/images/{fileName}";

            // Kitap nesnesini güncelleyin
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Dosyayı okuyup yanıt olarak döndürün
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/jpeg");

        }





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

       

        [HttpPost("BorrowBook/{employeeId}/{bookCopyId}")]
        public async Task<IActionResult> BorrowBook(string employeeId,int bookCopyId)
        {
            var employee = await _context.Employees.Include(m => m.BorrowedBooks).FirstOrDefaultAsync(m => m.Id == employeeId);

            if (employee == null)
            {
                return NotFound("Employee Not Found");
            }

            var bookCopy = await _context.BookCopies.FindAsync(bookCopyId);

            if (bookCopy == null || !bookCopy.IsAvailable)
            {
                return NotFound("Book copy not available");
            }

            if(employee.BorrowedBooks!=null && employee.BorrowedBooks.Count >= 5)
            {
                return BadRequest("You cannot borrow more than 5 books");
            }

            employee.BorrowedBooks.Add(bookCopy);
            bookCopy.IsAvailable = false;
            bookCopy.BorrowingEmployeeId = employeeId;

            _context.Employees.Update(employee);
            _context.BookCopies.Update(bookCopy);

            await _context.SaveChangesAsync(); //Veri tabanı değişikliklerini kaydetmek için kullanılır.

            return Ok("Book borrowed successfully");

        }

        [HttpPost("DeliverBook/{employeeId}/{bookCopyId}")]
        public async Task<IActionResult> DeliverBook(string employeeId,int bookCopyId, int ratingValue)
        {
            var employee = await _context.Employees.Include(m => m.BorrowedBooks).Include(m=>m.DeliveredBooks).FirstOrDefaultAsync(m => m.Id == employeeId);

            if (employee == null)
            {
                return NotFound("Employee Not Found");
            }

            var bookCopy = await _context.BookCopies.FindAsync(bookCopyId);

            if(bookCopy==null)
            {
                return NotFound("Book copy not avaliable");
            }

            if (bookCopy.IsAvailable)
            {
                return BadRequest("Book copy is already available");
            }

            if (!employee.BorrowedBooks.Contains(bookCopy))
            {
                return BadRequest("Book copy was not borrowed by this employee");
            }

            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.BookCopy.Id == bookCopyId);

            if (rating == null)
            {
                // If rating for this book copy doesn't exist, create a new one
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
                // Update existing rating
                rating.RatingSum += ratingValue;
                rating.RatingAmount++;
                // Update average rating
                rating.AverageRating = (double)rating.RatingSum / rating.RatingAmount;
                _context.Ratings.Update(rating);
            }

            employee.DeliveredBooks.Add(bookCopy);
            employee.BorrowedBooks.Remove(bookCopy);
            bookCopy.IsAvailable = true;
            bookCopy.DeliveringEmployeeId = employeeId;

            _context.Employees.Update(employee);
            _context.BookCopies.Update(bookCopy);

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





        private bool EmployeeExists(string id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
