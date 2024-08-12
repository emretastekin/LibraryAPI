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
    public class TranslatorsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public TranslatorsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Translators
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Translator>>> GetTranslator()
        {
          if (_context.Translator == null)
          {
              return NotFound();
          }
            return await _context.Translator.ToListAsync();
        }

        // GET: api/Translators/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Translator>> GetTranslator(int id)
        {
          if (_context.Translator == null)
          {
              return NotFound();
          }
            var translator = await _context.Translator.FindAsync(id);

            if (translator == null)
            {
                return NotFound();
            }

            return translator;
        }

        // PUT: api/Translators/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTranslator(int id, Translator translator)
        {
            if (id != translator.Id)
            {
                return BadRequest();
            }

            _context.Entry(translator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TranslatorExists(id))
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

        // POST: api/Translators
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<ActionResult<Translator>> PostTranslator(Translator translator)
        {
          if (_context.Translator == null)
          {
              return Problem("Entity set 'ApplicationContext.Translator'  is null.");
          }
            _context.Translator.Add(translator);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTranslator", new { id = translator.Id }, translator);
        }

        // DELETE: api/Translators/5
        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTranslator(int id)
        {
            if (_context.Translator == null)
            {
                return NotFound();
            }
            var translator = await _context.Translator.FindAsync(id);
            if (translator == null)
            {
                return NotFound();
            }

            _context.Translator.Remove(translator);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TranslatorExists(int id)
        {
            return (_context.Translator?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
