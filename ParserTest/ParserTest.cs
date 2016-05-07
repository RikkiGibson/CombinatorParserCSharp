using Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ParserTest
{
    public class ParserTest
    {
        [Fact]
        public void TestManyStrings()
        {
            Parser<string> foo = Parsers.StringParser("Hello");
            Parser<List<string>> manyHellos = Parsers.Many(foo);
            ParseResult<List<string>>? output = manyHellos("HelloHelloHello");
            Assert.NotNull(output);
            Assert.True(!output.Value.Remaining.Any());
        }
    }
}
