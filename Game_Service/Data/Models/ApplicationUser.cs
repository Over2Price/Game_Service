using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game_Service.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public string? DisplayName { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        public int? CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public virtual Country? Country { get; set; }

        public DateTime? DateOfBirth { get; set; }


        public UserStatus Status { get; set; } = UserStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public bool IsPublisherRepresentative { get; set; }

        public int? PublisherId { get; set; }

        [MaxLength(100)]
        public string? PublisherRole { get; set; }

        public bool IsPublisherVerified { get; set; }

        public DateTime? PublisherVerifiedAt { get; set; }

        [MaxLength(500)]
        public string? PublisherVerificationNote { get; set; }


        [ForeignKey(nameof(PublisherId))]
        public virtual GamePublisher? Publisher { get; set; }
    }

    public enum UserStatus
    {
        Active = 1,
        Suspended = 2,
        Banned = 3,
        Deleted = 4
    }
}
