using Game_Service.Data;
using System.ComponentModel.DataAnnotations;
namespace Game_Service.Data.Models
{
    public class GamePublisher
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string GamePublisherName { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
        [MaxLength(500)]
        public string? Website { get; set; }
        [MaxLength(100)]
        public string? Country { get; set; }
        public DateTime? FoundedDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
