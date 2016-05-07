using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public delegate ParseResult<T>? Parser<T>(string input);

    public static class Parsers
    {
        public static Parser<string> StringParser(string toParse)
        {
            return input =>
            {
                if (input.StartsWith(toParse))
                {
                    var remaining = input.Substring(input.Length);
                    return new ParseResult<string>(remaining, input);
                }
                else
                {
                    return null;
                }
            };
        }

        public static Parser<List<T>> Many<T>(Parser<T> p)
        {
            return input =>
            {
                var li = new List<T>();
                ParseResult<T>? result = p(input);
                while (result != null)
                {
                    li.Add(result.Value.Output);
                    result = p(result.Value.Remaining);
                }

                return new ParseResult<List<T>>(result?.Remaining ?? input, li);
            };
        }


    }
}
