using NUnit.Framework;
using System;
using TsonLibrary;

namespace TsonLibrary.Tests
{
    [TestFixture()]
    public class TsonTokenizerTests
    {
        static void AssertTokens(TsonTokenizer tokenizer, TsonToken[] expectedTokens)
        {
            TsonToken token = tokenizer.PeekNext();
            TsonToken expectedToken = expectedTokens[0];
            Assert.AreEqual(expectedToken.TokenType, token.TokenType);
            Assert.AreEqual(expectedToken.Location.Offset, token.Location.Offset);
            Assert.AreEqual(expectedToken.Data, token.Data);
            for (int i = 0; i < expectedTokens.Length; i++)
            {
                expectedToken = expectedTokens[i];
                token = tokenizer.Next();
                string s = String.Format("Token '{0}', Offset {1}, Data {2}", expectedToken.TokenType.ToString(), expectedToken.Location.Offset, expectedToken.Data == null ? "null" : expectedToken.Data);
                Assert.AreEqual(expectedToken.TokenType, token.TokenType, s);
                Assert.AreEqual(expectedToken.Location.Offset, token.Location.Offset, s);
                Assert.AreEqual(expectedToken.Data, token.Data, s);
            }
            token = tokenizer.Next();
            Assert.IsTrue(token.IsEnd);
            token = tokenizer.PeekNext();
            Assert.IsTrue(token.IsEnd);
        }

        [Test]
        public void TestEmptyString()
        {
            TsonTokenizer tokenizer = new TsonTokenizer("");

            Assert.IsTrue(tokenizer.Next().IsEnd);
        }

        [Test]
        public void TestAllTypes()
        {
            TsonTokenizer tokenizer = 
                new TsonTokenizer(
                     /*
                     0000000000111111111 12 22222222233333333334444 4 4444455555555556666666666777777777788888888889999999999
                     0123456789012345678 90 12345678901234567890123 4 5678901234567890123456789012345678901234567890123456789 */
                    "  {a:true,b:123,f:\"a\",g:[1,2], h:{a:1,b:2},\n\txyz:  space  }");

            TsonToken[] expectedTokens = {
                TsonToken.Whitespace(0, "  "),
                TsonToken.LeftCurlyBrace(2),
                TsonToken.String(3, "a"),
                TsonToken.Colon(4),
                TsonToken.String(5, "true"),
                TsonToken.Comma(9),
                TsonToken.String(10, "b"),
                TsonToken.Colon(11),
                TsonToken.String(12, "123"),
                TsonToken.Comma(15),
                TsonToken.String(16, "f"),
                TsonToken.Colon(17),
                TsonToken.String(18, "\"a\""),
                TsonToken.Comma(21),
                TsonToken.String(22, "g"),
                TsonToken.Colon(23),
                TsonToken.LeftSquareBrace(24),
                TsonToken.String(25, "1"),
                TsonToken.Comma(26),
                TsonToken.String(27, "2"),
                TsonToken.RightSquareBrace(28),
                TsonToken.Comma(29),
                TsonToken.Whitespace(30, " "),
                TsonToken.String(31, "h"),
                TsonToken.Colon(32),
                TsonToken.LeftCurlyBrace(33),
                TsonToken.String(34, "a"),
                TsonToken.Colon(35),
                TsonToken.String(36, "1"),
                TsonToken.Comma(37),
                TsonToken.String(38, "b"),
                TsonToken.Colon(39),
                TsonToken.String(40, "2"),
                TsonToken.RightCurlyBrace(41),
                TsonToken.Comma(42),
                TsonToken.Whitespace(43, "\n\t"),
                TsonToken.String(45, "xyz"),
                TsonToken.Colon(48),
                TsonToken.Whitespace(49, "  "),
                TsonToken.String(51, "space"),
                TsonToken.Whitespace(56, "  "),
                TsonToken.RightCurlyBrace(58),
                TsonToken.End(59)
            };

            AssertTokens(tokenizer, expectedTokens);
        }

        [Test]
        public void TestComments()
        {
            TsonTokenizer tokenizer = 
                new TsonTokenizer(
                     /*
                     000000 00 0 01111111111 2 22222 2222333 33 33 333444444444455555555556666666666777777777788888888889999999999
                     012345 67 8 90123456789 0 12345 6789012 34 56 789012345678901234567890123456789012345678901234567890123456789 */
                    "# xxx\n{\n\ta:123 #yyy\n\tb : \"abc#def\"\n}\n# zzz");

            TsonToken[] expectedTokens = {
                TsonToken.Comment(0, "# xxx"),
                TsonToken.Whitespace(5, "\n"),
                TsonToken.LeftCurlyBrace(6),
                TsonToken.Whitespace(7, "\n\t"),
                TsonToken.String(9, "a"),
                TsonToken.Colon(10),
                TsonToken.String(11, "123"),
                TsonToken.Whitespace(14, " "),
                TsonToken.Comment(15, "#yyy"),
                TsonToken.Whitespace(19, "\n\t"),
                TsonToken.String(21, "b"),
                TsonToken.Whitespace(22, " "),
                TsonToken.Colon(23),
                TsonToken.Whitespace(24, " "),
                TsonToken.String(25, "\"abc#def\""),
                TsonToken.Whitespace(34, "\n"),
                TsonToken.RightCurlyBrace(35),
                TsonToken.Whitespace(36, "\n"),
                TsonToken.Comment(37, "# zzz"),
                TsonToken.End(42)
            };

            AssertTokens(tokenizer, expectedTokens);
        }
    }
}

