using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Exceptions
{
    [Serializable]
    class AllValuesGuessedException : Exception
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
