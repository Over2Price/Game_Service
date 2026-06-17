using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game_Service.Data.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        public DateTime? ReleaseDate { get; set; }
        [MaxLength(500)]
        public string? CoverImageUrl { get; set; }
        [MaxLength(500)]
        public string? TrailerUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int? PublisherId { get; set; }
        [ForeignKey(nameof(PublisherId))]
        public virtual GamePublisher? Publisher { get; set; }
        public virtual ICollection<GameCategory> GameCategories { get; set; } = new List<GameCategory>();
        public virtual ICollection<GameReview> GameReviews { get; set; } = new List<GameReview>();
    }
}
