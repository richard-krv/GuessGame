namespace Ric.GuessGame.Utils
{
    public interface ILogger
    {
        void AddLogItem(string format, params object[] args);
    }
}
