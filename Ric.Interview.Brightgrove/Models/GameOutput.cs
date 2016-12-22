using Ric.Interview.Brightgrove.FruitBasket.Presentation;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    internal class GameOutput: IGameOutput
    {
        public int SecretValue { get; private set; }
        public Player WinnerPlayer { get { return mi.GetWinnerPlayer(SecretValue); } }
        public int NumberOfAttempts { get { return mi.GetNumberOfAttempts(WinnerPlayer); } }
        private MaintenanceInfo mi;

        public GameOutput(IGameResolver gr, IMaintenanceInfo minfo)
        {
            SecretValue = gr.SecretValue;
            mi = minfo as MaintenanceInfo;
        }
    }
}
