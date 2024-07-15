using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models
{
    public class Library
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string LibraryName { get; set; } = "";

        [StringLength(200)]
        public string Address { get; set; } = "";


        [Phone]
        [StringLength(15, MinimumLength = 7)]
        [Column(TypeName = "varchar(15)")]
        public string? PhoneNumber { get; set; }

        public List<Room>? Rooms { get; set; }
        public List<Book>? Books { get; set; }
        public List<BookCopy>? BookCopies { get; set; }

        public List<Asset>? Assets { get; set; }

        public List<BookCopyLibrary>? BookCopyLibraries { get; set; } // BookCopyLibrary ilişkisi

        public List<BookLibrary>? BookLibraries { get; set; }


    }
}