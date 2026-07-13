namespace Json.Tests;

[TestClass]
[TestCategory("Lexer")]
public sealed class LexerTests
{
    [TestMethod]
    public void UnexpectedCharacterTest()
    {
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("(5 + 10) * 2.5"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("Hello world!"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("true false null \"apples\" ?"));
    }

    [TestMethod]
    public void StringTest()
    {
        var tokens = JsonLexer.Tokenise(" \t\n  \"Apples\"    \r\n    ");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.String, tokens[0].Kind);

        // Whitespace before and after the opening and closing quotation marks should be skipped.
        // Opening and closing quotation marks should be ignored.
        Assert.AreEqual("Apples", tokens[0].Value);
    }

    [TestMethod]
    public void EmptyStringTest()
    {
        var tokens = JsonLexer.Tokenise("\"\"");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.String, tokens[0].Kind);
        Assert.AreEqual("", tokens[0].Value);
    }

    [TestMethod]
    public void EscapeSequenceStringTest()
    {
        var tokens = JsonLexer.Tokenise("\" \\\\ \\/ \\b \\f \\n \\r \\t \\u002F Apples \"");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.String, tokens[0].Kind);
        Assert.AreEqual(" \\\\ \\/ \\b \\f \\n \\r \\t \\u002F Apples ", tokens[0].Value);
    }

    [TestMethod]
    public void UnterminatedStringTest()
    {
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("\"Apples"));
    }

    [TestMethod]
    public void ValidLiteralTest()
    {
        var tokensA = JsonLexer.Tokenise("  \t\t\t    true          \r\n");
        Assert.HasCount(1, tokensA);
        Assert.AreEqual(JsonTokenKind.True, tokensA[0].Kind);

        // Whitespace before and after should be skipped.
        Assert.AreEqual("true", tokensA[0].Value);


        var tokensB = JsonLexer.Tokenise("  \t \r    false   \n\n");
        Assert.HasCount(1, tokensB);
        Assert.AreEqual(JsonTokenKind.False, tokensB[0].Kind);

        // Whitespace before and after should be skipped.
        Assert.AreEqual("false", tokensB[0].Value);


        var tokensC = JsonLexer.Tokenise("null");
        Assert.HasCount(1, tokensC);
        Assert.AreEqual(JsonTokenKind.Null, tokensC[0].Kind);
        Assert.AreEqual("null", tokensC[0].Value);
    }

    [TestMethod]
    public void InvalidLiteralTest()
    {
        Assert.Throws<FormatException>(() =>JsonLexer.Tokenise("trUE"));
        Assert.Throws<FormatException>(() =>JsonLexer.Tokenise("FaLsE"));
        Assert.Throws<FormatException>(() =>JsonLexer.Tokenise("nullable"));
    }
}