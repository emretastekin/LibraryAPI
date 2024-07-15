﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
	public class Donor
	{
		public int Id { get; set; }

		[Required]
		[StringLength(800)]
        public string Name { get; set; } = "";

		[Phone]
		[StringLength(15,MinimumLength =7)]
		[Column(TypeName ="varchar(15)")]
		public string? Phone { get; set; }

		public DateTime DonationDate { get; set; }

		[EmailAddress]
		[StringLength(320,MinimumLength =3)]
		[Column(TypeName ="varchar(320)")]
		public string? Email { get; set; }

		[JsonIgnore]
		public List<Book>? Books { get; set; }

		public List<BookDonor>? BookDonors { get; set; }



    }
}

