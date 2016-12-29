namespace Ric.GuessGame.Presentation
{
    public interface IGameResolver
    {
        int SecretValue { get; }
        int MaxAttempts { get; }
        int MaxMilliseconds { get; }
    }
}
