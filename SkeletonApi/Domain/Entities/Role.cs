using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkeletonApi.Domain.Entities
{
    public class Role : IdentityRole
    {
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        [NotMapped]
        public ICollection<UserRole> UserRoles { get; set; }
        [NotMapped]
        public UserRole UserRole { get; set; }
        public ICollection<Permission> Permissions { get; } = new List<Permission>();

    }
}
