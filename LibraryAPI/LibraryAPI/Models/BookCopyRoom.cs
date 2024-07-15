using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class BookCopyRoom
    {
        public int BookCopiesId { get; set; }
        
        public int RoomsId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BookCopiesId))]
        public BookCopy? BookCopy { get; set; }  //Bu özellik BookCopyRoom sınıfında bir BookCopy nesnesini tutabilir veya bu nesne null olabilir.

        [JsonIgnore]
        [ForeignKey(nameof(RoomsId))]
        public Room? Room { get; set; }
    }
}
