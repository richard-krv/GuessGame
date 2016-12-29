using System;
using System.Runtime.Serialization;

namespace Ric.GuessGame.Exceptions
{
    [Serializable]
    public class IncorrectAbstractFactorySettingsException : GuessGameExceptionBase
    {
        public IncorrectAbstractFactorySettingsException()
        {
        }

        public IncorrectAbstractFactorySettingsException(string message) : base(message)
        {
        }

        public IncorrectAbstractFactorySettingsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IncorrectAbstractFactorySettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}