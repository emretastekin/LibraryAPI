using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryAPI.Models
{
	public class Author
	{
        public long AuthorId { get; set; }

        [Required]
        [StringLength(800)]
        public string FullName { get; set; } = "";  //NULL olamaz

        [Column(TypeName ="nvarchar(1000)")]
        public string? Biography { get; set; }

        [Range(-4000,2100)]
        public short BirthDate { get; set; }

        [Range(-400,2100)]
        public short? DeathDate { get; set; }  //DeadTime Null olabilir

        public List<AuthorBook>? AuthorBooks { get; set; } //Yazdığı kitapların listesi
    }
}

