using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class BookCopy
    {
        public int Id { get; set; }

        [StringLength(6,MinimumLength =3)]
        [Column(TypeName ="varchar(6)")]
        public string LocationShelf { get; set; } = "";
        public bool IsAvailable { get; set; }
        public bool IsDamaged { get; set; }
        public DateTime? BorrowDate { get; set; }

        public int BooksId { get; set; }

        public int PublisherId { get; set; }

        public int TranslatorId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(TranslatorId))]
        public Translator? Translator { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(PublisherId))]
        public Publisher? Publisher { get; set; }

        [JsonIgnore]
        public Location? Location { get; set; }

        public List<Library>? Libraries { get; set; }

        public List<Room>? Rooms { get; set; }

        public List<Asset>? Assets { get; set; }

        public List<BookCopyLibrary>? BookCopyLibraries { get; set; } // BookCopyLibrary ilişkisi

        public List<BookCopyRoom>? BookCopyRooms { get; set; }

    }


}
