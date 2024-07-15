using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class BookDonor
    {
        public int BooksId { get; set; }
        public int DonorsId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(DonorsId))]
        public Donor? Donor { get; set; }

    }
}
