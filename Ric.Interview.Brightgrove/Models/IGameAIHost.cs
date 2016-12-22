using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using System;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public interface IGameAIHost : IDisposable
    {
        IGameOutput GameOutput { get; }
        GameLog GameLog { get; }
        void StartGame();
        bool IsCancellationRequested { get; }
    }
}
