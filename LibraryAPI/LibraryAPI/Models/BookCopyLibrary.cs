using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class BookCopyLibrary
    {
        public int BookCopiesId { get; set; }
        
        public int LibrariesId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BookCopiesId))]
        public BookCopy? BookCopy { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(LibrariesId))]
        public Library? Library { get; set; }
    }
}
