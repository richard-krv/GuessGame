using System;
using System.Diagnostics;

namespace Ric.Interview.Brightgrove.FruitBasket.Utils
{
    public class LoggerAspect : ILogger
    {
        public void AddLogItem(string format, params object[] args)
        {
            var stackTrace = new StackTrace();
            var callerName = stackTrace.GetFrame(1).GetMethod().Name;

            Console.WriteLine(format, args);
        }
    }
}
