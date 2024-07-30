using System.ComponentModel.DataAnnotations.Schema;

namespace SkeletonApi.Domain.Common.Abstracts
{
    public abstract class BaseManyToMany
    {
        private readonly List<BaseEvent> _domainEvents = new();

        [Column("created_by")]
        public Guid? CreatedBy { get; set; }

        [Column("update_by")]
        public Guid? UpdatedBy { get; set; }

        [Column("deleted_by")]
        public Guid? DeletedBy { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [NotMapped]
        public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);

        public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}