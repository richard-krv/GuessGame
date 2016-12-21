using Ric.Interview.Brightgrove.FruitBasket.Presentation;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    internal class GameOutput: IGameOutput
    {
        public int SecretValue { get; private set; }
        public Player WinnerPlayer { get; private set; }
        public int NumberOfAttempts { get; private set; }

        public GameOutput(IGameResolver gr, MaintenanceInfo mi)
        {
            SecretValue = gr.SecretValue;
            WinnerPlayer = mi.GetWinnerPlayer(SecretValue);
            NumberOfAttempts = mi.GetNumberOfAttempts(WinnerPlayer);
        }
    }
}
