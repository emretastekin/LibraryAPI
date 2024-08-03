using System;
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Services
{
	public class AuthorService
	{
		private readonly ApplicationContext _context;

		public AuthorService(ApplicationContext context)
		{
			_context = context;
		}

		public ActionResult<Author> GetAuthorById(long id)
		{
            var author = _context.Authors!
                .Include(a => a.AuthorBooks)!
                .ThenInclude(ab => ab.Book)
                .FirstOrDefault(a => a.AuthorId == id);

			

           
            return author;
        }
	}
}

