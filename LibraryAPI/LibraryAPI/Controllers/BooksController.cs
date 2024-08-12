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
using Microsoft.VisualStudio.Web.CodeGeneration;
using QRCoder;
using SkiaSharp;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BooksController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
          if (_context.Books == null)
          {
              return NotFound();
          }
            return await _context.Books.Include(b=>b.Publisher).Include(b=>b.AuthorBooks)!.ThenInclude(a=>a.Author).ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
          if (_context.Books == null)
          {
              return NotFound();
          }
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        //[Authorize(Roles ="Worker")]
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            AuthorBook authorBook;
            
            //Category category = book.SubCategories[0].Category;
            /*
            if (User.HasClaim("Category", category.Name) == false)
            {
                return Unauthorized();
            }
            */
          if (_context.Books == null)
          {
              return Problem("Entity set 'ApplicationContext.Books'  is null.");
          }
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            if (book.AuthorIds != null)
            {
                foreach(long authorId in book.AuthorIds)
                {
                    authorBook = new AuthorBook();
                    authorBook.AuthorsAuthorId = authorId;
                    authorBook.BooksId = book.Id;
                    _context.AuthorBooks!.Add(authorBook);
                }
                _context.SaveChanges();
            }

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("upload-cover-image/{bookId}")]
        public async Task<IActionResult> UploadCoverImage(int bookId, IFormFile coverImage)
        {
            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Ki�iyi bul
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }

            // Dosya yolunu belirleyin (�rne�in: wwwroot/images/{fileName})
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            // Klas�r�n var olup olmad���n� kontrol edin ve gerekiyorsa olu�turun
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = coverImage.FileName;
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Dosyay� kaydedin
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await coverImage.CopyToAsync(stream);
            }

            // Ki�inin CoverImageUrl �zelli�ini g�ncelleyin
            book.CoverImageUrl = $"/images/{fileName}";

            // Ki�i nesnesini g�ncelleyin
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Dosyay� okuyup yan�t olarak d�nd�r�n
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/jpeg");
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete("remove-cover-image/{bookId}")]
        public async Task<IActionResult> RemoveCoverImage(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }

            // Eski kapak resminin dosya yolunu belirleyin
            var oldFileName = Path.GetFileName(book.CoverImageUrl);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", oldFileName);

            // Dosya varsa, dosyay� kald�r�n
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Kapak resmini kald�r�n
            book.CoverImageUrl = null;

            // �ye nesnesini g�ncelleyin
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("QRCodeGenerator/{bookId}")]
        public async Task<IActionResult> Generate(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.AuthorBooks)
                .ThenInclude(ab => ab.Author)
                .Include(b=>b.Publisher)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                return NotFound("Book not found.");
            }

            var qrCodeContent = $"Kitap ID: {book.Id}\n" +
                                $"Kitap Ad�: {book.Title}\n" +
                                $"Yazarlar: {string.Join(", ", book.AuthorBooks.Select(ab => ab.Author.FullName))}\n" +
                                $"Yay�nevi: {book.Publisher?.Name}\n" +
                                $"Yay�n Tarihi: {book.PublishingYear}";

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

            // QR kodu PNG olarak döndür
            return File(qrCodeBytes, "image/png");
        }




        // DELETE: api/Books/5
        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
