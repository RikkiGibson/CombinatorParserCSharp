using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser
{
    public delegate ParseResult<T>? Parser<T>(string input);

    public static class Parsers
    {
        public static readonly Parser<string> WhitespaceParser = RegexParser(@"\s+");
        public static readonly Parser<string> EOFParser = RegexParser("$");
        
        public static Parser<string> StringParser(string toParse)
        {
            return input =>
            {
                if (input.StartsWith(toParse))
                {
                    var remaining = input.Substring(toParse.Length);
                    return new ParseResult<string>(remaining, toParse);
                }
                else
                {
                    return null;
                }
            };
        }

        public static Parser<string> RegexParser(string pattern)
        {
            return input =>
            {
                // Patterns need to match the beginning of the string
                var match = Regex.Match(input, "^" + pattern);
                if (match.Success)
                {
                    var remaining = input.Substring(match.Length);
                    return new ParseResult<string>(remaining, match.Value);
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
                string remaining = input;
                for (var result = p(input); result != null; result = p(result.Value.Remaining))
                {
                    li.Add(result.Value.Output);
                    remaining = result.Value.Remaining;
                }

                return new ParseResult<List<T>>(remaining, li);
            };
        }

        public static Parser<T> Lookahead<T>(Parser<T> p)
        {
            return input =>
            {
                var output = p(input);
                return output == null
                    ? (ParseResult<T>?)null
                    : new ParseResult<T>(input, output.Value.Output);
            };
        }

        public static Parser<List<T>> ManyTill<T,U>(Parser<T> manyParser, Parser<U> tillParser)
        {
            return input =>
            {
                var li = new List<T>();
                var remaining = input;

                // Don't have a cow about this
                while (true)
                {
                    var tillResult = tillParser(remaining);
                    if (tillResult.HasValue)
                    {
                        return new ParseResult<List<T>>(tillResult.Value.Remaining, li);
                    }

                    var manyResult = manyParser(remaining);
                    if (manyResult.HasValue)
                    {
                        remaining = manyResult.Value.Remaining;
                        li.Add(manyResult.Value.Output);
                    }
                    else
                    {
                        return null;
                    }
                }
            };
        }

        public static Parser<T> Choice<T>(params Parser<T>[] parsers)
        {
            return input =>
            {
                foreach (var parser in parsers)
                {
                    var parseResult = parser(input);
                    if (parseResult.HasValue)
                    {
                        return parseResult;
                    }
                }
                return null;
            };
        }

        /// <summary>
        /// Composes two parsers, discarding the result of the second parser.
        /// </summary>
        public static Parser<T> SequenceRight<T,U>(Parser<T> p, Parser<U> q)
        {
            return input =>
            {
                var pResult = p(input);
                if (pResult == null)
                {
                    return null;
                }

                var qResult = q(pResult.Value.Remaining);
                if (qResult == null)
                {
                    return null;
                }

                return new ParseResult<T>(qResult.Value.Remaining, pResult.Value.Output);
            };
        }

        /// <summary>
        /// Sequence two parsers, discarding the left result.
        /// </summary>
        public static Parser<U> SequenceLeft<T,U>(Parser<T> p, Parser<U> q)
        {
            return input =>
            {
                var pResult = p(input);
                if (pResult == null)
                {
                    return null;
                }

                var qResult = q(pResult.Value.Remaining);
                if (qResult == null)
                {
                    return null;
                }

                return new ParseResult<U>(pResult.Value.Remaining, qResult.Value.Output);
            };
        }
    }
}
