using System;
using System.Runtime.Serialization;

namespace Ric.GuessGame.Exceptions
{
    public abstract class GuessGameExceptionBase : Exception
    {
        public GuessGameExceptionBase()
        {
        }

        public GuessGameExceptionBase(string message) : base(message)
        {
        }

        public GuessGameExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GuessGameExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
