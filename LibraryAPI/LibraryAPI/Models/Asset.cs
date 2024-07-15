using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class Asset //Demirbaş  Asset, kütüphanedeki fiziksel veya dijital
                       //varlıkları temsil eder (örneğin, kitaplar, dergiler, ekipmanlar vb.).
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(20)]
        public string AssetNumber { get; set; } = "";

        [StringLength(500)]
        public string Description { get; set; } = "";

        public bool IsAvailable { get; set; }

        public Library? Library { get; set; }  //Library? Library { get; set; }: Bu, demirbaşın ait olduğu kütüphaneyi temsil eder.Yani, bu demirbaş hangi kütüphanede bulunuyor.


        public Book? Book { get; set; }

        public BookCopy? BookCopy { get; set; }

        
    }
}
