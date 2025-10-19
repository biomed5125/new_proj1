using HMS.SharedKernel.Abstractions;
namespace HMS.SharedKernel.Base
{
    public abstract class BaseEntity<TId> : IEntity<TId>, IAuditable, ISoftDeletable
    {
        public TId Id { get; protected set; } = default!;
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
