using System;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>  // IdentityDbContext (macte bu yeterlidir)
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
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
        public DbSet<BookLanguage>? BookLanguages { get; set; }
        public DbSet<BookSubCategory>? BookSubCategories { get; set; }
        public DbSet<BookCopy>? BookCopies { get; set; }
        public DbSet<Asset>? Asset { get; set; }
        public DbSet<Room>? Room { get; set; }
        public DbSet<Library>? Library { get; set; }
        public DbSet<Translator>? Translator { get; set; }
        public DbSet<BookCopyLibrary>? BookCopyLibraries { get; set; }    
        public DbSet<BookCopyRoom>? BookCopyRoom { get; set; }
        public DbSet<BookDonor>? BookDonor { get; set; }  
        public DbSet<BookLibrary>? BookLibrary { get; set; }
        public DbSet<BookRoom>? BookRoom { get; set; }
        public DbSet<BookTranslator>? BookTranslator { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            // BookCopyLibrary tablosunu yapılandırma
            modelBuilder.Entity<BookCopyLibrary>()
                .ToTable("BookCopyLibraries")
                .HasKey(bcl => new { bcl.BookCopiesId, bcl.LibrariesId }); // Birleşik birincil anahtar

            modelBuilder.Entity<BookCopyLibrary>()
                .HasOne(bcl => bcl.BookCopy)
                .WithMany(bc => bc.BookCopyLibraries)
                .HasForeignKey(bcl => bcl.BookCopiesId);

            modelBuilder.Entity<BookCopyLibrary>()
                .HasOne(bcl => bcl.Library)
                .WithMany(l => l.BookCopyLibraries)
                .HasForeignKey(bcl => bcl.LibrariesId);

            modelBuilder.Entity<BookCopyRoom>()
                .ToTable("BookCopyRooms")
                .HasKey(bcr => new { bcr.BookCopiesId, bcr.RoomsId });

            modelBuilder.Entity<BookCopyRoom>()
                .HasOne(bcr => bcr.BookCopy)
                .WithMany(bc => bc.BookCopyRooms)
                .HasForeignKey(bcr => bcr.BookCopiesId);

            modelBuilder.Entity<BookCopyRoom>()
                .HasOne(bcr => bcr.Room)
                .WithMany(r => r.BookCopyRooms)
                .HasForeignKey(bcr => bcr.RoomsId);

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

            modelBuilder.Entity<BookLibrary>()
                .ToTable("BookLibraries")
                .HasKey(bl => new { bl.BooksId, bl.LibrariesId });

            modelBuilder.Entity<BookLibrary>()
                .HasOne(bl => bl.Book)
                .WithMany(b => b.BookLibraries)
                .HasForeignKey(bl => bl.BooksId);

            modelBuilder.Entity<BookLibrary>()
                .HasOne(bl => bl.Library)
                .WithMany(l => l.BookLibraries)
                .HasForeignKey(bl => bl.LibrariesId);

            modelBuilder.Entity<BookRoom>()
                .ToTable("BookRooms")
                .HasKey(br => new { br.BooksId, br.RoomsId });

            modelBuilder.Entity<BookRoom>()
                .HasOne(br => br.Book)
                .WithMany(b => b.BookRooms)
                .HasForeignKey(br => br.BooksId);

            modelBuilder.Entity<BookRoom>()
                .HasOne(br => br.Room)
                .WithMany(r => r.BookRooms)
                .HasForeignKey(br => br.RoomsId);


            
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


            base.OnModelCreating(modelBuilder);
        }

    }
}
