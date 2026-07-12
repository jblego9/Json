namespace Json.Tests;

[TestClass]
[TestCategory("Lexer")]
public sealed class LexerTests
{
    [TestMethod]
    public void UnexpectedCharacterTest()
    {
        string sourceA = "(5 + 10) * 2.5";
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise(sourceA));

        string sourceB = "Hello world!";
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise(sourceB));

        string sourceC = "true false null \"apples\" ?";
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise(sourceC));
    }

    [TestMethod]
    public void ValidStringTest()
    {
        string source = " \t\n  \"Apples\"    \r\n    ";
        var tokens = JsonLexer.Tokenise(source);

        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.String, tokens[0].Kind);

        // Whitespace before and after the opening and closing quotation marks should be skipped.
        // Opening and closing quotation marks should be ignored.
        Assert.AreEqual("Apples", tokens[0].Value);
    }

    [TestMethod]
    public void UnterminatedStringTest()
    {
        string source = "\"Apples";
        Assert.Throws<FormatException>(() =>JsonLexer.Tokenise(source));
        Assert.Throws<FormatException>(() =>JsonLexer.Tokenise(source));
    }

    [TestMethod]
    public void ValidLiteralTest()
    {
        string sourceA = "  \t\t\t    true          \r\n";
        var tokensA = JsonLexer.Tokenise(sourceA);

        Assert.HasCount(1, tokensA);
        Assert.AreEqual(JsonTokenKind.True, tokensA[0].Kind);

        // Whitespace before and after should be skipped.
        Assert.AreEqual("true", tokensA[0].Value);


        string sourceB = "  \t \r    false   \n\n";
        var tokensB = JsonLexer.Tokenise(sourceB);

        Assert.HasCount(1, tokensB);
        Assert.AreEqual(JsonTokenKind.False, tokensB[0].Kind);

        // Whitespace before and after should be skipped.
        Assert.AreEqual("false", tokensB[0].Value);


        string sourceC = "null";
        var tokensC = JsonLexer.Tokenise(sourceC);

        Assert.HasCount(1, tokensC);
        Assert.AreEqual(JsonTokenKind.Null, tokensC[0].Kind);
        Assert.AreEqual("null", tokensC[0].Value);
    }

    [TestMethod]
    public void InvalidLiteralTest()
    {
        string sourceA = "trUE";
        Assert.Throws<FormatException>(() =>JsonLexer.Tokenise(sourceA));

        string sourceB = "FaLsE";
        Assert.Throws<FormatException>(() =>JsonLexer.Tokenise(sourceB));

        string sourceC = "nullable";
        Assert.Throws<FormatException>(() =>JsonLexer.Tokenise(sourceC));
    }
}