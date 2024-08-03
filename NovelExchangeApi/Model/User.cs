using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NovelExchangeApi.Model;

[Table("user")]
public class User
{
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Column("first_name")]
    public string? FirstName { get; set; }
    
    [Column("last_name")]
    public string? LastName { get; set; }
    
    [Required]
    [EmailAddress]
    [Column("email")]
    public string Email { get; set; } = null!;
    
    [Required]
    [Column("password")]
    public string? Password { get; set; } = null!;
    
    [Required]
    [Column("role")]
    public string[]? Role { get; set; } = Array.Empty<string>();
    
    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    // Navigation properties
    public ICollection<Book> Books { get; set; } = new List<Book>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
