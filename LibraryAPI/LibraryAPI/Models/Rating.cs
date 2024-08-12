using System;
namespace LibraryAPI.Models
{
	public class Rating
	{
		public int Id { get; set; }

		public int RatingSum { get; set; }

		public int RatingAmount { get; set; }

		public double AverageRating { get; set; }

        public string? MemberId { get; set; }

		public string? EmployeeId { get; set; }

		public int? DonorId1 { get; set; }

        public BookCopy? BookCopy { get; set; }


    }
}

