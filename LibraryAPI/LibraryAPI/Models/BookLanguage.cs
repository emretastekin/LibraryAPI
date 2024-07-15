using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
	public class BookLanguage
	{
		public int BooksId { get; set; }
		public string LanguagesCode { get; set; } = "";

		[JsonIgnore]
		[ForeignKey(nameof(BooksId))]
		public Book? Book { get; set; }

		[JsonIgnore]
		[ForeignKey(nameof(LanguagesCode))]
		public Language? Language { get; set; }


	}
}

