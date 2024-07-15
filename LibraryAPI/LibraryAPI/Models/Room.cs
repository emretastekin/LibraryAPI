using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = "";

        public int Capacity { get; set; }

        public Library? Library { get; set; }

        public List<Book>? Books { get; set; }

        public List<BookCopy>? BookCopies { get; set; }

        public List<BookCopyRoom>? BookCopyRooms { get; set; }

        public List<BookRoom>? BookRooms { get; set; }
        
    }
}
