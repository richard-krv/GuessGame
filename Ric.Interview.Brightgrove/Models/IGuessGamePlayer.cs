namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public interface IGuessGamePlayer
    {
        int Guess();
        string Name { get; }
        PlayerType Type { get; }
    }
}
