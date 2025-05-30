using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Exceptions
{
    public class InvalidAnalysisRequestException : DomainException
    {
        public InvalidAnalysisRequestException(string message) : base(message) { }
        public InvalidAnalysisRequestException() : base("Invalid code analysis request") { }
    }
}
