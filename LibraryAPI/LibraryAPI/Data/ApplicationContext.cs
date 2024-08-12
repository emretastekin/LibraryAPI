using System;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>  // IdentityDbContext (macte bu yeterlidir)
    {
        public readonly IHttpContextAccessor httpContextAccessor;

        public ApplicationContext(DbContextOptions<ApplicationContext> options,IHttpContextAccessor httpContextAccessor) : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public DbSet<Location>? Locations { get; set; }
        public DbSet<Language>? Languages { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<SubCategory>? SubCategories { get; set; }
        public DbSet<Publisher>? Publishers { get; set; }
        public DbSet<Author>? Authors { get; set; }
        public DbSet<Book>? Books { get; set; }
        public DbSet<AuthorBook>? AuthorBooks { get; set; }
        public DbSet<Member>? Members { get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<Donor>? Donors { get; set; }
        public DbSet<BookLanguage>? BookLanguage { get; set; }
        public DbSet<BookSubCategory>? BookSubCategory { get; set; }
        public DbSet<BookCopy>? BookCopies { get; set; }
        public DbSet<Translator>? Translator { get; set; }
        public DbSet<BookDonor>? BookDonor { get; set; }  
        public DbSet<BookTranslator>? BookTranslator { get; set; }
        public DbSet<FavoriteBook>? FavoriteBooks { get; set; }
        public DbSet<Penalty>? Penalties { get; set; }
        public DbSet<Rating>? Ratings { get; set; }
        public DbSet<ApplicationUser>? ApplicationUsers { get; set; }







        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.IdNumber)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISSN)
                .IsUnique();

            modelBuilder.Entity<Donor>()
                .HasIndex(d => d.Phone)
                .IsUnique();

            modelBuilder.Entity<Donor>()
                .HasIndex(d => d.Email)
                .IsUnique();

            modelBuilder.Entity<Language>()
                .HasIndex(l => l.Code)
                .IsUnique();

            modelBuilder.Entity<Language>()
                .HasIndex(l => l.Name)
                .IsUnique();

            modelBuilder.Entity<Location>()
                .HasIndex(lo => lo.Shelf)
                .IsUnique();

            modelBuilder.Entity<Publisher>()
                .HasIndex(lo => lo.Phone)
                .IsUnique();

            modelBuilder.Entity<Publisher>()
                .HasIndex(lo => lo.EMail)
                .IsUnique();

            modelBuilder.Entity<Translator>()
                .HasIndex(lo => lo.Email)
                .IsUnique();

            modelBuilder.Entity<Translator>()
                .HasIndex(lo => lo.Phone)
                .IsUnique();



            // AuthorBook tablosunu yapılandırma
            modelBuilder.Entity<AuthorBook>()
                .ToTable("AuthorBooks") // Tablo adını açıkça belirtin
                .HasKey(ab => new { ab.AuthorsAuthorId, ab.BooksId });

            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Author)
                .WithMany(a => a.AuthorBooks)
                .HasForeignKey(ab => ab.AuthorsAuthorId);

            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Book)
                .WithMany(b => b.AuthorBooks)
                .HasForeignKey(ab => ab.BooksId);

            // BookLanguage tablosunu yapılandırma
            modelBuilder.Entity<BookLanguage>()
                .ToTable("BookLanguages") // Tablo adını açıkça belirtin
                .HasKey(bl => new { bl.BooksId, bl.LanguagesCode });

            modelBuilder.Entity<BookLanguage>()
                .HasOne(bl => bl.Book)
                .WithMany(b => b.BookLanguages)
                .HasForeignKey(bl => bl.BooksId);

            modelBuilder.Entity<BookLanguage>()
                .HasOne(bl => bl.Language)
                .WithMany(l => l.BookLanguages)
                .HasForeignKey(bl => bl.LanguagesCode);

            modelBuilder.Entity<BookSubCategory>()
                .ToTable("BookSubCategories")                
                .HasKey(cl => new { cl.BooksId, cl.SubCategoriesId });

            modelBuilder.Entity<BookSubCategory>()
                .HasOne(cl => cl.Book)
                .WithMany(c => c.BookSubCategories)
                .HasForeignKey(cl => cl.BooksId);

            modelBuilder.Entity<BookSubCategory>()
                .HasOne(cl => cl.SubCategory)
                .WithMany(l => l.BookSubCategories)
                .HasForeignKey(cl => cl.SubCategoriesId);

            modelBuilder.Entity<BookCopy>()
                .ToTable("BookCopies")
                .HasKey(bc => bc.Id);

            modelBuilder.Entity<BookCopy>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.BookCopies)
                .HasForeignKey(bc => bc.BooksId)
                .OnDelete(DeleteBehavior.Restrict); // veya NoAction

            modelBuilder.Entity<BookCopy>()
                .HasOne(bc => bc.Publisher)
                .WithMany()
                .HasForeignKey(bc => bc.PublisherId)
                .OnDelete(DeleteBehavior.Restrict); // veya NoAction

            modelBuilder.Entity<BookCopy>()
                .HasOne(bc => bc.Location)
                .WithMany()
                .HasForeignKey(bc => bc.LocationShelf)
                .OnDelete(DeleteBehavior.Restrict); // veya NoAction

            modelBuilder.Entity<FavoriteBook>()
                .ToTable("FavoriteBooks")
                .HasKey(fb => fb.Id);

            modelBuilder.Entity<FavoriteBook>()
                .HasOne(fb => fb.BookCopy)
                .WithMany()
                .HasForeignKey(fb => fb.BookCopyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteBook>()
                .HasOne(fb => fb.Member)
                .WithMany(m=>m.FavoriteBooks)
                .HasForeignKey(fb => fb.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FavoriteBook>()
                .HasOne(fb => fb.Employee)
                .WithMany(e=>e.FavoriteBooks)
                .HasForeignKey(fb => fb.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Member>()
               .HasMany(e => e.BorrowedBooks)
               .WithOne(b => b.BorrowingMember)
               .HasForeignKey(b => b.BorrowingMemberId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Member>()
                .HasMany(e => e.DeliveredBooks)
                .WithOne(b => b.DeliveringMember)
                .HasForeignKey(b => b.DeliveringMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            


            modelBuilder.Entity<Employee>()
                .HasMany(e => e.BorrowedBooks)
                .WithOne(b => b.BorrowingEmployee)
                .HasForeignKey(b => b.BorrowingEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.DeliveredBooks)
                .WithOne(b => b.DeliveringEmployee)
                .HasForeignKey(b => b.DeliveringEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);







            modelBuilder.Entity<BookDonor>()
                .ToTable("BookDonors")
                .HasKey(bd => new { bd.BooksId, bd.DonorsId });

            modelBuilder.Entity<BookDonor>()
                .HasOne(bd => bd.Book)
                .WithMany(b => b.BookDonors)
                .HasForeignKey(bd => bd.BooksId);

            modelBuilder.Entity<BookDonor>()
                .HasOne(bd => bd.Donor)
                .WithMany(d => d.BookDonors)
                .HasForeignKey(bd => bd.DonorsId);







            modelBuilder.Entity<BookTranslator>()
                .ToTable("BookTranslators")
                .HasKey(bt => new { bt.BooksId, bt.TranslatorId });

            modelBuilder.Entity<BookTranslator>()
                .HasOne(bt => bt.Book)
                .WithMany(b => b.BookTranslators)
                .HasForeignKey(bt => bt.BooksId);

            modelBuilder.Entity<BookTranslator>()
                .HasOne(bt => bt.Translator)
                .WithMany(r => r.BookTranslators)
                .HasForeignKey(bt => bt.TranslatorId);

            modelBuilder.Entity<Publisher>()
                .HasIndex(b => b.Phone)
                .IsUnique();


            base.OnModelCreating(modelBuilder);
        }

    }
}
