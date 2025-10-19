namespace HMS.Communication.Domain.CommEntities
{
    public enum OutboxState { Pending = 0, Dispatched = 1, Acked = 2, Failed = 9 }

}
