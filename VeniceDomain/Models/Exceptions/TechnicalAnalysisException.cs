using System;

namespace VeniceDomain.Models.Exceptions
{
    public class TechnicalAnalysisException : Exception
    {
        public TechnicalAnalysisException() : base()
        {
        }

        public TechnicalAnalysisException(string message) : base(message)
        {
        }
    }
}
