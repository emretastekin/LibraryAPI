using System;
namespace LibraryAPI.Models
{
    public class FavoriteBook
    {
        public string? Id { get; set; }

        public int BookCopyId { get; set; }

        public string? MemberId { get; set; }

        public string? EmployeeId { get; set; }


        public DateTime? FavoriteDate { get; set; }

        public Member? Member { get; set; }

        public Employee? Employee { get; set; }

        public BookCopy? BookCopy { get; set; }
    }
}