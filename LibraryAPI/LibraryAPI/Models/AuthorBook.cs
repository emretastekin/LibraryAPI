using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
	public class AuthorBook
	{
		public long AuthorsAuthorId { get; set; }

        public int BooksId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(AuthorsAuthorId))]
		public Author? Author { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BooksId))]
		public Book? Book { get; set; }
	}
}

