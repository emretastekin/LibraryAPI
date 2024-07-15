using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class BookRoom
    {
        public int BooksId { get; set; }
        public int RoomsId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(RoomsId))]
        public Room? Room { get; set; }
    }
}
