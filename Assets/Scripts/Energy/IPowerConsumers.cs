public interface IPowerConsumer
{
    string DisplayName { get; }
    int PowerCost { get; }
    bool IsPowered { get; }
    void SetPowered(bool powered);
}