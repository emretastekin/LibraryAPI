using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class Book
    {
        public int Id { get; set; }             //Int mi Long mu? Veri tiplerinin saklayabileceği maksimum değerlere göre belirlenir. 
                                                // ekstradan Id ile birlikte ISBN numarasıda özellik olarak verilmelidir.Çünkü bir kitabın adeti birden fazla olabilir
        [StringLength(13, MinimumLength = 10)]                                         // ama eğer aynı yayın evinden ise ISBN leri aynı olur. Bundan dolayı kitap üzerinde işlem yapmak Id özelliği belirtilmelidir. 
        [Column(TypeName = "varchar(13)")]
        public string? ISBN { get; set; }       //(?) işareti var ise null olabilir.

        [StringLength(8)]
        [Column(TypeName ="varchar(8)")]
        public string? ISSN { get; set; }  //ISSN, süreli yayınlar, dergiler ve gazeteler gibi devam eden seriler için kullanılan bir numaradır.

        [Required]
        [StringLength(2000)]
        public string Title { get; set; } = ""; //Title null değer olamaz.
        /*
        public Book()
        {
            Title = "";
        }
        */

        [Range(1, short.MaxValue)]
        public short PageCount { get; set; }

        [Range(-4000, 2100)]
        public short PublishingYear { get; set; }

        [StringLength(5000)]
        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int PrintCount { get; set; }

        public bool Banned { get; set; }

        [Range(0, 5)]
        public float Rating { get; set; }

        [Required]
        [StringLength(200)]
        public string Series { get; set; } = "";

        public int PublisherId { get; set; }

        public float PhysicalQuality { get; set; }

        public string Situation { get; set; } = "";

        [StringLength(500)]
        public string? CoverImageUrl { get; set; } // Resim URL'si veya yolu

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = "";

        [StringLength(100)]
        public string? Edition { get; set; }

        [NotMapped]
        public List<long>? AuthorIds { get; set; }

        [NotMapped]
        public List<long>? DonorIds { get; set; }

        [StringLength(6, MinimumLength = 3)]
        [Column(TypeName = "varchar(6)")]
        public string LocationShelf { get; set; } = "";

        public List<AuthorBook>? AuthorBooks { get; set; }  //Bir kitabın birden fazla yazarı olabilir.(Birden fazla yazar olsa bile her yerde (publisherlarda) aynı sayı kadar yazar olur)

        [JsonIgnore]
        [ForeignKey(nameof(PublisherId))]
        public Publisher? Publisher { get; set; }   //Bir kitap birden fazla yayınevi tarafından çıkarabilir.Ondan dolayı List kullanmadık.Publisherların birbirinden farklı ISBN'leri vardır. Her kitap kaydının kendine özel publisherı vardır.


        [JsonIgnore]
        public List<Language>? Languages { get; set; }

        public List<SubCategory>? SubCategories { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(LocationShelf))]
        public Location? Location { get; set; }


        [JsonIgnore]
        public List<Donor>? Donors { get; set; }

        [JsonIgnore]
        public List<Translator>? Translators { get; set; }

        [JsonIgnore]
        public List<BookLanguage>? BookLanguages { get; set; }

        [JsonIgnore]
        public List<BookSubCategory>? BookSubCategories { get; set; }

        [JsonIgnore]
        public List<BookCopy>? BookCopies { get; set; }



        [JsonIgnore]
        public List<BookDonor>? BookDonors { get; set; }


        [JsonIgnore]
        public List<BookTranslator>? BookTranslators { get; set; }



    }

}

