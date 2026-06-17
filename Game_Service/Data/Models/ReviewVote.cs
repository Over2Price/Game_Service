using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game_Service.Data.Models
{
    public class ReviewVote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int ReviewId { get; set; }

        public bool IsHelpful { get; set; } = true;

        public DateTime VotedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(ReviewId))]
        public virtual GameReview Review { get; set; } = null!;
    }
}