using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public struct ParseResult<T>
    {
        public ParseResult(string remaining, T output)
        {
            Remaining = remaining;
            Output = output;
        }
        
        public string Remaining { get; private set; }
        public T Output { get; private set; }
    }
}
