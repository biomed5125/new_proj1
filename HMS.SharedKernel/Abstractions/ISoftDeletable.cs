namespace HMS.SharedKernel.Abstractions
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}