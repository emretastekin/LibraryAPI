using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class BookTranslator
    {
        public int BooksId { get; set; }
        
        public int TranslatorId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(TranslatorId))]
        public Translator? Translator { get; set; }
    }
}
