using Ric.GuessGame.Presentation;
using System;

namespace Ric.GuessGame.Models
{
    public interface IGameAIHost : IDisposable
    {
        IGameOutput GameOutput { get; }
        void StartGame();
        int TotalAttemptsCount { get; }
    }
}
