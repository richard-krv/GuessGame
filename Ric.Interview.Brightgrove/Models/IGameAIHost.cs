using System;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public interface IGameAIHost: IDisposable
    {
        void StartGame();
    }
}
