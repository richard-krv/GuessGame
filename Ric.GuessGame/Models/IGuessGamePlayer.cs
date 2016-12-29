namespace Ric.GuessGame.Models
{
    public interface IGuessGamePlayer
    {
        int Guess();
        string Name { get; }
        PlayerType Type { get; }
    }
}
