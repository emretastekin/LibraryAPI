using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Models
{
    public class ApplicationUser: IdentityUser
    {
        public long IdNumber { get; set; }
        public string Name { get; set; } = "";
        public string? MiddleName { get; set; }
        public string? FamilyName { get; set; }
        public string Address { get; set; } = "";
        public bool Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public byte Status { get; set; }
        [NotMapped]
        public string? Password { get; set; }

        [NotMapped]  //Veri tabanına kaydedilmicektir.
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

    }

    public class Member 
    {
        [Key]
        public string Id { get; set; } = "";

        [ForeignKey(nameof(Id))]
        public ApplicationUser? ApplicationUser { get; set; }

        public byte EducationalDegree { get; set; }

        public byte BorrowedDayLimit { get; set; }

        [StringLength(500)]
        public string? CoverImageUrl { get; set; } // Resim URL'si veya yolu

        [JsonIgnore]
        public List<BookCopy>? BorrowedBooks { get; set; }  //Üyenin kiraladığı kitap kopyaları

        [JsonIgnore]
        public List<BookCopy>? DeliveredBooks { get; set; }  // Employee'nin teslim ettiği kitapların kopyaları

        [JsonIgnore]
        public List<FavoriteBook>? FavoriteBooks { get; set; }

        [JsonIgnore]
        public List<Penalty>? Penalties { get; set; }

        [JsonIgnore]
        public List<Rating>? Ratings { get; set; }

    }

    public class Employee
    {
        [Key]
        public string Id { get; set; } = "";

        [ForeignKey(nameof(Id))]   //Employee  Id ile yabancı anahtar ilişkisi oluşturulur.
                                   //Bu, Employee ve ApplicationUser tablosu
                                   //arasında bir yabancı anahtar ilişkisi oluşturur.

        public ApplicationUser? ApplicationUser { get; set; }

        public string Title { get; set; } = "";
        public float Salary { get; set; }
        public string Department { get; set; } = "";
        public string? Shift { get; set; }

        [StringLength(500)]
        public string? CoverImageUrl { get; set; } // Resim URL'si veya yolu

        [JsonIgnore]
        public List<BookCopy>? BorrowedBooks { get; set; }  //Employee'nin kiraladığı kitaplar

        [JsonIgnore]
        public List<BookCopy>? DeliveredBooks { get; set; }  // Employee'nin teslim ettiği kitapların kopyaları

        [JsonIgnore]
        public List<FavoriteBook>? FavoriteBooks { get; set; }

        [JsonIgnore]
        public List<Penalty>? Penalties { get; set; }

        [JsonIgnore]
        public List<Rating>? Ratings { get; set; }



        

    }
}
