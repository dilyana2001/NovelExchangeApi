using Microsoft.EntityFrameworkCore;
using NovelExchangeApi.Model;

namespace NovelExchangeApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Review> Reviews { get; set; }
}
