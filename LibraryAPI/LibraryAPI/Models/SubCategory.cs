using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
	public class SubCategory
	{
        public short Id { get; set; }

        //[Required]
        [StringLength(800)]
        [Column(TypeName = "varchar(800)")]
        public string Name { get; set; } = "";


        public short CategoryID { get; set; }  //Sonradan eklendi

        [JsonIgnore]
        [ForeignKey(nameof(CategoryID))]   //Sonradan eklendi
        public Category? Category { get; set; }

        [JsonIgnore]
        public List<Book>? Books { get; set; }

        [JsonIgnore]
        public List<BookSubCategory>? BookSubCategories { get; set; }
    }
}

