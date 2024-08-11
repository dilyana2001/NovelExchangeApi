using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NovelExchangeApi.Model;

[Table("review")]
public class Review
{
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [Column("title")]
    public string? Title { get; set; } = null!;
    
    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("book_id")]
    public Guid BookId { get; set; }
}
