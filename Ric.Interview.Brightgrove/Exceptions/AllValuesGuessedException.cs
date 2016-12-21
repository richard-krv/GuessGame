using System;
using System.Runtime.Serialization;

namespace Ric.Interview.Brightgrove.FruitBasket.Exceptions
{
    [Serializable]
    public class AllValuesGuessedException : GuessGameExceptionBase
    {
        public AllValuesGuessedException()
        {
        }

        public AllValuesGuessedException(string message) : base(message)
        {
        }

        public AllValuesGuessedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AllValuesGuessedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
