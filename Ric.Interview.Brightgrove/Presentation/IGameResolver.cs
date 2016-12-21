namespace Ric.Interview.Brightgrove.FruitBasket.Presentation
{
    public interface IGameResolver
    {
        int SecretValue { get; }
        int MaxAttempts { get; }
        int MaxMilliseconds { get; }
    }
}
