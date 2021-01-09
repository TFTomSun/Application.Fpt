using System;

namespace Thomas.Apis.Core.DotNet
{
    public class ApiException : ApplicationException
    {
        public bool Optimized { get; }

        public ApiException(string message, Exception inner, bool optimized) : base(message, inner)
        {
            Optimized = optimized;
        }

        public string OriginalStackTrace => base.StackTrace;

        public override string StackTrace
        {
            get
            {
                //if (Optimized)
                //{
                //    var result = StackTraceParser.OptimizeStackTrace(OriginalStackTrace, false, false, true, false);
                //    return result;
                //}
                //else
                //{
                    return OriginalStackTrace;
                //}
            }
        }
    }
}
