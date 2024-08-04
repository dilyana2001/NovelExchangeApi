using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NovelExchangeApi.Model;

[Table("author")]
public class Author
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("first_name")]
    public string? FirstName { get; set; } = null!;

    [Column("last_name")]
    public string? LastName { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();

    public ICollection<User> Users { get; set; } = new List<User>();
}
