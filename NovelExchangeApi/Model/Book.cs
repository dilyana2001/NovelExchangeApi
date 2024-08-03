using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NovelExchangeApi.Model;

[Table("book")]
public class Book
{
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [Column("title")]
    public string? Title { get; set; } = null!;
    
    [Column("volume")]
    public string? Volume { get; set; }
    
    [Column("release_year")]
    public DateTime? ReleaseYear { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
    
    [Column("description")]
    public string? Description { get; set; }
    
    [Column("genre")]
    public string? Genre { get; set; }
    
    [Column("series")]
    public string? Series { get; set; }

    // Foreign key for Author
    [Column("author_id")]
    public Guid AuthorId { get; set; }
    
    [ForeignKey("AuthorId")]
    public Author Author { get; set; } = null!;

    // Navigation property for Reviews
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
