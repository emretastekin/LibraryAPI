using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
	public class Penalty
	{
		[Key]
		public int PenaltyId { get; set; }

		public string? MemberId { get; set; }

		public string? EmployeeId { get; set; }

		public DateTime PenaltyDate { get; set; }

		public decimal PenaltyAmount { get; set; }

	}
}

