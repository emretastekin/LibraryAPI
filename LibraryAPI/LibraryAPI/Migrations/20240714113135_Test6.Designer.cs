﻿// <auto-generated />
using System;
using LibraryAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LibraryAPI.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240714113135_Test6")]
    partial class Test6
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.32")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BookDonor", b =>
                {
                    b.Property<int>("BooksId")
                        .HasColumnType("int");

                    b.Property<int>("DonorsId")
                        .HasColumnType("int");

                    b.HasKey("BooksId", "DonorsId");

                    b.HasIndex("DonorsId");

                    b.ToTable("BookDonor");
                });

            modelBuilder.Entity("BookLanguage", b =>
                {
                    b.Property<int>("BooksId")
                        .HasColumnType("int");

                    b.Property<string>("LanguagesCode")
                        .HasColumnType("char(3)");

                    b.HasKey("BooksId", "LanguagesCode");

                    b.HasIndex("LanguagesCode");

                    b.ToTable("BookLanguage");
                });

            modelBuilder.Entity("BookSubCategory", b =>
                {
                    b.Property<int>("BooksId")
                        .HasColumnType("int");

                    b.Property<short>("SubCategoriesId")
                        .HasColumnType("smallint");

                    b.HasKey("BooksId", "SubCategoriesId");

                    b.HasIndex("SubCategoriesId");

                    b.ToTable("BookSubCategory");
                });

            modelBuilder.Entity("BookTranslator", b =>
                {
                    b.Property<int>("BooksId")
                        .HasColumnType("int");

                    b.Property<int>("TranslatorsId")
                        .HasColumnType("int");

                    b.HasKey("BooksId", "TranslatorsId");

                    b.HasIndex("TranslatorsId");

                    b.ToTable("BookTranslator");
                });

            modelBuilder.Entity("LibraryAPI.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FamilyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Gender")
                        .HasColumnType("bit");

                    b.Property<long>("IdNumber")
                        .HasColumnType("bigint");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("Status")
                        .HasColumnType("tinyint");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("LibraryAPI.Models.Author", b =>
                {
                    b.Property<long>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AuthorId"), 1L, 1);

                    b.Property<string>("Biography")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<short>("BirthDate")
                        .HasColumnType("smallint");

                    b.Property<short?>("DeathDate")
                        .HasColumnType("smallint");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(800)
                        .HasColumnType("nvarchar(800)");

                    b.HasKey("AuthorId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("LibraryAPI.Models.AuthorBook", b =>
                {
                    b.Property<long>("AuthorsAuthorId")
                        .HasColumnType("bigint");

                    b.Property<int>("BooksId")
                        .HasColumnType("int");

                    b.HasKey("AuthorsAuthorId", "BooksId");

                    b.HasIndex("BooksId");

                    b.ToTable("AuthorBooks", (string)null);
                });

            modelBuilder.Entity("LibraryAPI.Models.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Banned")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .HasMaxLength(13)
                        .HasColumnType("varchar(13)");

                    b.Property<string>("LocationShelf")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("varchar(6)");

                    b.Property<short>("PageCount")
                        .HasColumnType("smallint");

                    b.Property<float>("PhysicalQuality")
                        .HasColumnType("real");

                    b.Property<int>("PrintCount")
                        .HasColumnType("int");

                    b.Property<int>("PublisherId")
                        .HasColumnType("int");

                    b.Property<short>("PublishingYear")
                        .HasColumnType("smallint");

                    b.Property<float>("Rating")
                        .HasColumnType("real");

                    b.Property<string>("Series")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Situation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("Id");

                    b.HasIndex("LocationShelf");

                    b.HasIndex("PublisherId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("LibraryAPI.Models.BookCopy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("BooksId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("BorrowDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDamaged")
                        .HasColumnType("bit");

                    b.Property<string>("LocationShelf")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("varchar(6)");

                    b.Property<string>("LocationShelf1")
                        .HasColumnType("varchar(6)");

                    b.Property<int>("PublisherId")
                        .HasColumnType("int");

                    b.Property<int?>("PublisherId1")
                        .HasColumnType("int");

                    b.Property<int>("TranslatorId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BooksId");

                    b.HasIndex("LocationShelf");

                    b.HasIndex("LocationShelf1");

                    b.HasIndex("PublisherId");

                    b.HasIndex("PublisherId1");

                    b.HasIndex("TranslatorId");

                    b.ToTable("BookCopies", (string)null);
                });

            modelBuilder.Entity("LibraryAPI.Models.BookLanguage", b =>
                {
                    b.Property<int>("BooksId")
                        .HasColumnType("int");

                    b.Property<string>("LanguagesCode")
                        .HasColumnType("char(3)");

                    b.HasKey("BooksId", "LanguagesCode");

                    b.HasIndex("LanguagesCode");

                    b.ToTable("BookLanguages", (string)null);
                });

            modelBuilder.Entity("LibraryAPI.Models.BookSubCategory", b =>
                {
                    b.Property<int>("BooksId")
                        .HasColumnType("int");

                    b.Property<short>("SubCategoriesId")
                        .HasColumnType("smallint");

                    b.HasKey("BooksId", "SubCategoriesId");

                    b.HasIndex("SubCategoriesId");

                    b.ToTable("BookSubCategories", (string)null);
                });

            modelBuilder.Entity("LibraryAPI.Models.Category", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<short>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(800)
                        .HasColumnType("varchar(800)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("LibraryAPI.Models.Donor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DonationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(320)
                        .HasColumnType("varchar(320)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(800)
                        .HasColumnType("nvarchar(800)");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .HasColumnType("varchar(15)");

                    b.HasKey("Id");

                    b.ToTable("Donors");
                });

            modelBuilder.Entity("LibraryAPI.Models.Employee", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Salary")
                        .HasColumnType("real");

                    b.Property<string>("Shift")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("LibraryAPI.Models.Language", b =>
                {
                    b.Property<string>("Code")
                        .HasMaxLength(3)
                        .HasColumnType("char(3)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Code");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("LibraryAPI.Models.Location", b =>
                {
                    b.Property<string>("Shelf")
                        .HasMaxLength(6)
                        .HasColumnType("varchar(6)");

                    b.HasKey("Shelf");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("LibraryAPI.Models.Member", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte>("BorrowedDayLimit")
                        .HasColumnType("tinyint");

                    b.Property<byte>("EducationalDegree")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("LibraryAPI.Models.Publisher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ContactPerson")
                        .HasMaxLength(800)
                        .HasColumnType("nvarchar(800)");

                    b.Property<string>("EMail")
                        .HasMaxLength(320)
                        .HasColumnType("varchar(320)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(800)
                        .HasColumnType("nvarchar(800)");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .HasColumnType("varchar(15)");

                    b.Property<string>("Place")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Publishers");
                });

            modelBuilder.Entity("LibraryAPI.Models.SubCategory", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<short>("Id"), 1L, 1);

                    b.Property<short>("CategoryID")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(800)
                        .HasColumnType("varchar(800)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryID");

                    b.ToTable("SubCategories");
                });

            modelBuilder.Entity("LibraryAPI.Models.Translator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Biography")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(746)
                        .HasColumnType("nvarchar(746)");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.ToTable("Translator");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("BookDonor", b =>
                {
                    b.HasOne("LibraryAPI.Models.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.Donor", null)
                        .WithMany()
                        .HasForeignKey("DonorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookLanguage", b =>
                {
                    b.HasOne("LibraryAPI.Models.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.Language", null)
                        .WithMany()
                        .HasForeignKey("LanguagesCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookSubCategory", b =>
                {
                    b.HasOne("LibraryAPI.Models.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.SubCategory", null)
                        .WithMany()
                        .HasForeignKey("SubCategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookTranslator", b =>
                {
                    b.HasOne("LibraryAPI.Models.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.Translator", null)
                        .WithMany()
                        .HasForeignKey("TranslatorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LibraryAPI.Models.AuthorBook", b =>
                {
                    b.HasOne("LibraryAPI.Models.Author", "Author")
                        .WithMany("AuthorBooks")
                        .HasForeignKey("AuthorsAuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.Book", "Book")
                        .WithMany("AuthorBooks")
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Book");
                });

            modelBuilder.Entity("LibraryAPI.Models.Book", b =>
                {
                    b.HasOne("LibraryAPI.Models.Location", "Location")
                        .WithMany("Books")
                        .HasForeignKey("LocationShelf")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.Publisher", "Publisher")
                        .WithMany("Books")
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("LibraryAPI.Models.BookCopy", b =>
                {
                    b.HasOne("LibraryAPI.Models.Book", "Book")
                        .WithMany("BookCopies")
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationShelf")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.Location", null)
                        .WithMany("BookCopies")
                        .HasForeignKey("LocationShelf1");

                    b.HasOne("LibraryAPI.Models.Publisher", "Publisher")
                        .WithMany()
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.Publisher", null)
                        .WithMany("BookCopies")
                        .HasForeignKey("PublisherId1");

                    b.HasOne("LibraryAPI.Models.Translator", "Translator")
                        .WithMany("BookCopies")
                        .HasForeignKey("TranslatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Location");

                    b.Navigation("Publisher");

                    b.Navigation("Translator");
                });

            modelBuilder.Entity("LibraryAPI.Models.BookLanguage", b =>
                {
                    b.HasOne("LibraryAPI.Models.Book", "Book")
                        .WithMany("BookLanguages")
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.Language", "Language")
                        .WithMany("BookLanguages")
                        .HasForeignKey("LanguagesCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Language");
                });

            modelBuilder.Entity("LibraryAPI.Models.BookSubCategory", b =>
                {
                    b.HasOne("LibraryAPI.Models.Book", "Book")
                        .WithMany("BookSubCategories")
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.SubCategory", "SubCategory")
                        .WithMany("BookSubCategories")
                        .HasForeignKey("SubCategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("SubCategory");
                });

            modelBuilder.Entity("LibraryAPI.Models.Employee", b =>
                {
                    b.HasOne("LibraryAPI.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("LibraryAPI.Models.Member", b =>
                {
                    b.HasOne("LibraryAPI.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("LibraryAPI.Models.SubCategory", b =>
                {
                    b.HasOne("LibraryAPI.Models.Category", "Category")
                        .WithMany("SubCategories")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("LibraryAPI.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("LibraryAPI.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LibraryAPI.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("LibraryAPI.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LibraryAPI.Models.Author", b =>
                {
                    b.Navigation("AuthorBooks");
                });

            modelBuilder.Entity("LibraryAPI.Models.Book", b =>
                {
                    b.Navigation("AuthorBooks");

                    b.Navigation("BookCopies");

                    b.Navigation("BookLanguages");

                    b.Navigation("BookSubCategories");
                });

            modelBuilder.Entity("LibraryAPI.Models.Category", b =>
                {
                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("LibraryAPI.Models.Language", b =>
                {
                    b.Navigation("BookLanguages");
                });

            modelBuilder.Entity("LibraryAPI.Models.Location", b =>
                {
                    b.Navigation("BookCopies");

                    b.Navigation("Books");
                });

            modelBuilder.Entity("LibraryAPI.Models.Publisher", b =>
                {
                    b.Navigation("BookCopies");

                    b.Navigation("Books");
                });

            modelBuilder.Entity("LibraryAPI.Models.SubCategory", b =>
                {
                    b.Navigation("BookSubCategories");
                });

            modelBuilder.Entity("LibraryAPI.Models.Translator", b =>
                {
                    b.Navigation("BookCopies");
                });
#pragma warning restore 612, 618
        }
    }
}
