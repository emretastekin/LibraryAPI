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
using LibraryAPI.Services;

namespace LibraryAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="Employee")]  //En tepeye yerleştirdiğimiz yetkilendirme işlemi bu controller içindeki tüm işlemleri yapabilir.
    public class AuthorsController : ControllerBase
    {
        
        private readonly ApplicationContext _context;
        private readonly AuthorService _authorService;


        public AuthorsController(ApplicationContext context, AuthorService authorService)
        {
            _context = context;
            _authorService = authorService;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
          if (_context.Authors == null)
          {
              return NotFound();
          }
            return await _context.Authors.ToListAsync();
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public ActionResult<Author> GetAuthor(long id)
        {

            var author= _authorService.GetAuthorById(id);

            if (author == null)
            {
                return NotFound();


            }
            return Ok(author);



        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(long id, Author author)
        {
            if (id != author.AuthorId)
            {
                return BadRequest();
            }

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize(Roles ="Worker,Employee")]
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            /*
            if (User.IsInRole("Worker") == false)
            {
                return Unauthorized();
            }
            */
          if (_context.Authors == null)
          {
              return Problem("Entity set 'ApplicationContext.Authors'  is null.");
          }
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.AuthorId }, author);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("upload-cover-image/{authorId}")]
        public async Task<IActionResult> UploadCoverImage(long authorId, IFormFile coverImage)
        {
            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Kişiyi bul
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
            {
                return NotFound("Author not found.");
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
            author.CoverImageUrl = $"/images/{fileName}";

            // Kişi nesnesini güncelleyin
            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Dosyayı okuyup yanıt olarak döndürün
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/jpeg");
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete("remove-cover-image/{authorId}")]
        public async Task<IActionResult> RemoveCoverImage(long authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
            {
                return NotFound("Author not found.");
            }

            // Eski kapak resminin dosya yolunu belirleyin
            var oldFileName = Path.GetFileName(author.CoverImageUrl);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", oldFileName);

            // Dosya varsa, dosyayı kaldırın
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Kapak resmini kaldırın
            author.CoverImageUrl = null;

            // Üye nesnesini güncelleyin
            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Authors/5
        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(long id)
        {
            if (_context.Authors == null)
            {
                return NotFound();
            }
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(long id)
        {
            return (_context.Authors?.Any(e => e.AuthorId == id)).GetValueOrDefault();
        }
    }
}
