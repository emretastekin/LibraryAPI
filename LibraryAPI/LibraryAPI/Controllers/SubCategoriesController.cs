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
    public class SubCategoriesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public SubCategoriesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/SubCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetSubCategories()
        {
          if (_context.SubCategories == null)
          {
              return NotFound();
          }
            return await _context.SubCategories.ToListAsync();
        }

        // GET: api/SubCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubCategory>> GetSubCategory(short id)
        {
          if (_context.SubCategories == null)
          {
              return NotFound();
          }
            var subCategory = await _context.SubCategories.FindAsync(id);

            if (subCategory == null)
            {
                return NotFound();
            }

            return subCategory;
        }

        // PUT: api/SubCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubCategory(short id, SubCategory subCategory)
        {
            if (id != subCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(subCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubCategoryExists(id))
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

        // POST: api/SubCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<ActionResult<SubCategory>> PostSubCategory(SubCategory subCategory)
        {
            if (_context.SubCategories == null)
            {
                return Problem("Entity set 'ApplicationContext.SubCategories' is null.");
            }

            // Ayný CategoryID ve Name ile SubCategory olup olmadýðýný kontrol et
            var existingSubCategory = await _context.SubCategories
                .FirstOrDefaultAsync(sc => sc.Name == subCategory.Name && sc.CategoryID == subCategory.CategoryID);

            if (existingSubCategory != null)
            {
                return BadRequest($"SubCategory with Name {subCategory.Name} already exists in Category ID {subCategory.CategoryID}.");
            }

            // Yeni SubCategory ekle
            _context.SubCategories.Add(subCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubCategory", new { id = subCategory.Id }, subCategory);
        }


        // DELETE: api/SubCategories/5
        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(short id)
        {
            if (_context.SubCategories == null)
            {
                return NotFound();
            }
            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubCategoryExists(short id)
        {
            return (_context.SubCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
