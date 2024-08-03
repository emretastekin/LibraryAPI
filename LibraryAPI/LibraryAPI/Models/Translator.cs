using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class Translator
    {
        public int Id { get; set; }

        [Required]
        [StringLength(746)]
        public string FullName { get; set; } = "";

        [StringLength(500)]
        public string? Biography { get; set; }

        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }


        public List<Book>? Books { get; set; }

        [JsonIgnore]
        public List<BookCopy>? BookCopies { get; set; }

        [JsonIgnore]
        public List<BookTranslator>? BookTranslators { get; set; }


    }
}
