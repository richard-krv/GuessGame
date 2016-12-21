using System;
using System.Runtime.Serialization;

namespace Ric.Interview.Brightgrove.FruitBasket.Exceptions
{
    [Serializable]
    public class InvalidGameCompletionException : GuessGameExceptionBase
    {
        public InvalidGameCompletionException()
        {
        }

        public InvalidGameCompletionException(string message) : base(message)
        {
        }

        public InvalidGameCompletionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidGameCompletionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}