using Microsoft.EntityFrameworkCore;
using NovelExchangeApi.Model;

namespace NovelExchangeApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>()
            .HasMany(a => a.Users)
            .WithMany(u => u.Authors)
            .UsingEntity<Dictionary<string, object>>(
                "AuthorUser",
                j => j
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("user_id")
                    .HasPrincipalKey("Id"),
                j => j
                    .HasOne<Author>()
                    .WithMany()
                    .HasForeignKey("author_id")
                    .HasPrincipalKey("Id"),
                j =>
                {
                    j.HasKey("author_id", "user_id");
                    j.ToTable("author_user");
                }
            )
        ;

        modelBuilder.Entity<Book>()
            .HasMany(a => a.Users)
            .WithMany(u => u.Books)
            .UsingEntity<Dictionary<string, object>>(
                "BookUser",
                j => j
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("user_id")
                    .HasPrincipalKey("Id"),
                j => j
                    .HasOne<Book>()
                    .WithMany()
                    .HasForeignKey("book_id")
                    .HasPrincipalKey("Id"),
                j =>
                {
                    j.HasKey("book_id", "user_id");
                    j.ToTable("book_user");
                }
            )
        ;
    }
}
