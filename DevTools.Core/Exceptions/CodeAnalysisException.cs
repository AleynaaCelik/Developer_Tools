using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Exceptions
{
    public class CodeAnalysisException : DomainException
    {
        public CodeAnalysisException(string message) : base(message) { }
        public CodeAnalysisException(string message, Exception innerException) : base(message, innerException) { }
    }
}
