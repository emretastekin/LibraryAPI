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
        public string? LocationShelf { get; set; }

        public bool IsAvailable { get; set; }
        public bool IsDamaged { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? FavoritedDate { get; set; }

        public int BooksId { get; set; }

        public int PublisherId { get; set; }

        public int TranslatorId { get; set; }

        public string? BorrowingMemberId { get; set; }

        [ForeignKey(nameof(BorrowingMemberId))]
        public Member? BorrowingMember { get; set; }

        public string? DeliveringMemberId { get; set; }

        [ForeignKey(nameof(DeliveringMemberId))]
        public Member? DeliveringMember { get; set; }


        public string? BorrowingEmployeeId { get; set; }

        [ForeignKey(nameof(BorrowingEmployeeId))]
        public Employee? BorrowingEmployee { get; set; }

        public string? DeliveringEmployeeId { get; set; }

        [ForeignKey(nameof(DeliveringEmployeeId))]
        public Employee? DeliveringEmployee { get; set; }

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
        [ForeignKey(nameof(LocationShelf))]
        public Location? Location { get; set; }








    }


}
