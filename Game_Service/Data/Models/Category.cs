using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game_Service.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Связь многие-ко-многим с играми
        public virtual ICollection<GameCategory> GameCategories { get; set; } = new List<GameCategory>();
    }
}
