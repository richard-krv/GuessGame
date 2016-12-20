using System;
using System.Runtime.Serialization;

namespace Ric.Interview.Brightgrove.FruitBasket.Exceptions
{
    [Serializable]
    public class IncorrectAbstractFactorySettingsException : Exception
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