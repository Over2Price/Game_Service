using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game_Service.Data.Models
{
    public class GameReview
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public int GameId { get; set; }
        [Required]
        [Range(1, 10, ErrorMessage = "Оценка должна быть от 1 до 10")]
        public int Rating { get; set; }
        [MaxLength(200)]
        public string? Title { get; set; }
        [MaxLength(5000)]
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsRecommended { get; set; } = true;
        public int HelpfulCount { get; set; }
        public int NotHelpfulCount { get; set; }
        public ReviewStatus Status { get; set; } = ReviewStatus.Published;
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
        [ForeignKey(nameof(GameId))]
        public virtual Game Game { get; set; } = null!;
    }

    public enum ReviewStatus
    {
        Published = 1,
        Hidden = 2,
        Deleted = 3
    }
}