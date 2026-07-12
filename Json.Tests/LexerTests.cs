namespace Json.Tests;

[TestClass]
[TestCategory("Lexer")]
public sealed class LexerTests
{
    [TestMethod]
    public void StringTest()
    {
        string source = " \t\n  \"Apples\"    \r\n    ";
        var tokens = JsonLexer.Tokenise(source);

        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.String, tokens[0].Kind);

        // Whitespace before and after the opening and closing quotation marks should be skipped.
        // Opening and closing quotation marks should be ignored.
        Assert.AreEqual("Apples", tokens[0].Value);
    }
}