namespace HMS.SharedKernel.Abstractions
{
    public interface IEntity<TId>
    {
        TId Id { get; }
    }
}