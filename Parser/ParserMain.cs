using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class ParserMain
    {
        static void Main(string[] args)
        {
            TocsCommandLikeTest();
        }

        static void BasicHelloTest()
        {
            var helloParser = Parsers.StringParser("Hello");
            var manyHellos = Parsers.Many(helloParser);

            var result = manyHellos("HelloHelloHelloHello");
            Console.WriteLine(string.Join(", ", result.Value.Output));
            Console.Read();
        }

        static void TocsCommandLikeTest()
        {
            var sampleCommand = "A /1234 ALAN BRO T/help me out dawg";
            var parseResult = ParseTocsCommand(sampleCommand);
            if (parseResult == null)
            {
                Console.WriteLine("Couldn't parse: {0}", sampleCommand);
            }
            else
            {
                Console.WriteLine(parseResult.Value.Output);
            }
            Console.Read();
        }

        public class ACommandArgs
        {
            public string Command { get; set; }
            public string EventId { get; set; }
            public List<string> RadioNumbers { get; set; }
            public string OptionalText { get; set; }
        }

        static ParseResult<ACommandArgs>? ParseTocsCommand(string input)
        {
            var remaining = input;
            var commandParser = Parsers.SequenceRight(Parsers.RegexParser("A|a"), Parsers.WhitespaceParser);
            var commandParserResult = commandParser(remaining);
            if (commandParserResult == null)
            {
                return null;
            }
            remaining = commandParserResult.Value.Remaining;

            var eventIdParser = Parsers.SequenceRight(Parsers.RegexParser(@"\d{2}(T|t)\d{6}|/\d{4,6}"), Parsers.WhitespaceParser);
            var eventIdParseResult = eventIdParser(remaining);
            if (eventIdParseResult.HasValue)
            {
                remaining = eventIdParseResult.Value.Remaining;
            }
            
            var radioNumberParser = Parsers.SequenceRight(Parsers.RegexParser(@"\w+"), Parsers.WhitespaceParser);
            var radioNumbersParser = Parsers.ManyTill(radioNumberParser,
                Parsers.Choice(
                    Parsers.Lookahead(Parsers.RegexParser("(T|T)/")),
                    Parsers.WhitespaceParser,
                    Parsers.EOFParser
                ));
            var radioNumbersResult = radioNumbersParser(remaining);
            if (radioNumbersResult == null)
            {
                return null;
            }
            remaining = radioNumbersResult.Value.Remaining;
            
            var optionalTextParser = Parsers.SequenceLeft(Parsers.RegexParser("(T|t)/"), Parsers.RegexParser(@"[\w\s]+"));
            var optionalTextResult = optionalTextParser(remaining);
            if (optionalTextResult.HasValue)
            {
                remaining = optionalTextResult.Value.Remaining;
            }

            var commandArgs = new ACommandArgs
            {
                Command = commandParserResult.Value.Output,
                EventId = eventIdParseResult?.Output,
                RadioNumbers = radioNumbersResult.Value.Output,
                OptionalText = optionalTextResult?.Output
            };

            return new ParseResult<ACommandArgs>(remaining, commandArgs);
        }
    }
}
