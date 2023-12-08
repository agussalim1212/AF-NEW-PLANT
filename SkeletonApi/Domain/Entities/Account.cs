using SkeletonApi.Domain.Common.Abstracts;


namespace SkeletonApi.Domain.Entities
{
    public class Account : BaseAuditableEntity
    {
        public string? Username { get; set; }
        public string PhotoURL { get; set; }
       
    }
}
