using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game_Service.Data.Models
{
    public class GameCategory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(GameId))]
        public virtual Game Game { get; set; } = null!;
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

    }
}
