using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class BookLibrary
    {
        public int BooksId { get; set; }

        public int LibrariesId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(LibrariesId))]
        public Library? Library { get; set; }
    }
}
