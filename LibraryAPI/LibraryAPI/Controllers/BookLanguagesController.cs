﻿using System;
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
    public class BookLanguagesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookLanguagesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookLanguages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookLanguage>>> GetBookLanguages()
        {
          if (_context.BookLanguage == null)
          {
              return NotFound();
          }
            return await _context.BookLanguage.ToListAsync();
        }

        // GET: api/BookLanguages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookLanguage>> GetBookLanguage(int id)
        {
          if (_context.BookLanguage == null)
          {
              return NotFound();
          }
            var bookLanguage = await _context.BookLanguage.FindAsync(id);

            if (bookLanguage == null)
            {
                return NotFound();
            }

            return bookLanguage;
        }

        // PUT: api/BookLanguages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookLanguage(int id, BookLanguage bookLanguage)
        {
            if (id != bookLanguage.BooksId)
            {
                return BadRequest();
            }

            _context.Entry(bookLanguage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookLanguageExists(id))
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

        // POST: api/BookLanguages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookLanguage>> PostBookLanguage(BookLanguage bookLanguage)
        {
          if (_context.BookLanguage == null)
          {
              return Problem("Entity set 'ApplicationContext.BookLanguages'  is null.");
          }
            _context.BookLanguage.Add(bookLanguage);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookLanguageExists(bookLanguage.BooksId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookLanguage", new { id = bookLanguage.BooksId }, bookLanguage);
        }

        // DELETE: api/BookLanguages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookLanguage(int id)
        {
            if (_context.BookLanguage == null)
            {
                return NotFound();
            }
            var bookLanguage = await _context.BookLanguage.FindAsync(id);
            if (bookLanguage == null)
            {
                return NotFound();
            }

            _context.BookLanguage.Remove(bookLanguage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookLanguageExists(int id)
        {
            return (_context.BookLanguage?.Any(e => e.BooksId == id)).GetValueOrDefault();
        }
    }
}
