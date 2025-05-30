using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Exceptions
{
    public class UserLimitExceededException : DomainException
    {
        public UserLimitExceededException(string message) : base(message) { }
        public UserLimitExceededException() : base("User API usage limit exceeded") { }
    }
}
