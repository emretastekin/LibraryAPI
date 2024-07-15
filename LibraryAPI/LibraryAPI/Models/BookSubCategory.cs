using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
	public class BookSubCategory
	{
		public int BooksId { get; set; }
		public short SubCategoriesId { get; set; }

												//JsonIgnore Kullanımı: Eğer bir özelliğin önüne [JsonIgnore]
                                                //eklediyseniz, bu özellik JSON serileştirilmesi sırasında
                                                //göz ardı edilir. Bu nedenle, Book ve SubCategory gibi
        [JsonIgnore]							//özelliklerde [JsonIgnore] kullanıyorsanız, Swagger'da görünmeyeceklerdir.
        [ForeignKey(nameof(BooksId))]
		public Book? Book { get; set; }

		[JsonIgnore]
		[ForeignKey(nameof(SubCategoriesId))]
		public SubCategory? SubCategory { get; set; }
	}
}

