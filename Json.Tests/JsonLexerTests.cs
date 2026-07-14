namespace Json.Tests;

/// <summary>
/// Ensures unexpected characters and malformed tokens are detected.
/// <para>Ensures the correct token kinds are produced.</para>
/// <para>Ensures tokens have their valid and correct values.</para>
/// </summary>
[TestClass]
[TestCategory("JsonLexer")]
public sealed class JsonLexerTests
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
    public void PositiveIntegerNumberTest()
    {
        var tokens = JsonLexer.Tokenise("500");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.Number, tokens[0].Kind);
        Assert.AreEqual("500", tokens[0].Value);
    }

    [TestMethod]
    public void NegativeIntegerNumberTest()
    {
        var tokens = JsonLexer.Tokenise("-500");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.Number, tokens[0].Kind);
        Assert.AreEqual("-500", tokens[0].Value);
    }

    [TestMethod]
    public void PositiveWithFractionNumberTest()
    {
        var tokens = JsonLexer.Tokenise("2.5");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.Number, tokens[0].Kind);
        Assert.AreEqual("2.5", tokens[0].Value);
    }

    [TestMethod]
    public void NegativeWithFractionNumberTest()
    {
        var tokens = JsonLexer.Tokenise("-2.5");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.Number, tokens[0].Kind);
        Assert.AreEqual("-2.5", tokens[0].Value);
    }

    [TestMethod]
    public void ZeroWithFractionNumberTest()
    {
        var tokens = JsonLexer.Tokenise("0.5");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.Number, tokens[0].Kind);
        Assert.AreEqual("0.5", tokens[0].Value);
    }

    [TestMethod]
    public void InvalidFractionNumberTest()
    {
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise(".5"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("-.5"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("."));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("-."));
    }

    [TestMethod]
    public void InvalidNegativeNumberTest()
    {
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("-"));
    }

    [TestMethod]
    public void PositiveExponentNumberTest()
    {
        var tokens = JsonLexer.Tokenise("1E+100");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.Number, tokens[0].Kind);
        Assert.AreEqual("1E+100", tokens[0].Value);
    }

    [TestMethod]
    public void NegativeExponentNumberTest()
    {
        var tokens = JsonLexer.Tokenise("1E-100");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.Number, tokens[0].Kind);
        Assert.AreEqual("1E-100", tokens[0].Value);
    }

    [TestMethod]
    public void InvalidExponentNumberTest()
    {
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("1E-"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("1E+"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("E+100"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("E-100"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("E+"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("E-"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("E"));
        Assert.Throws<FormatException>(() => JsonLexer.Tokenise("1Ee+100"));
    }

    [TestMethod]
    public void ComplexNumberTest()
    {
        var tokens = JsonLexer.Tokenise("-22.5E+1628");
        Assert.HasCount(1, tokens);
        Assert.AreEqual(JsonTokenKind.Number, tokens[0].Kind);
        Assert.AreEqual("-22.5E+1628", tokens[0].Value);
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

    [TestMethod]
    public void ArrayTest()
    {
        var tokens = JsonLexer.Tokenise("[ 1, -2.5, 3E+100, true, false, null, \"Apples\" ]");
        Assert.HasCount(15, tokens);
        Assert.AreEqual(JsonTokenKind.OpeningBracket, tokens[0].Kind);
        Assert.AreEqual(JsonTokenKind.ClosingBracket, tokens[14].Kind);
        Assert.AreEqual(JsonTokenKind.Comma, tokens[2].Kind);
        Assert.AreEqual(JsonTokenKind.Comma, tokens[10].Kind);
        Assert.AreEqual(JsonTokenKind.String, tokens[13].Kind);
        Assert.AreEqual("3E+100", tokens[5].Value);
        Assert.AreEqual("Apples", tokens[13].Value);
    }

    [TestMethod]
    public void ObjectTest()
    {
        var tokens = JsonLexer.Tokenise("{ \"damage\" : 50, \"range\": 1000, \"health\":100, \"firerate\" :2 }");
        Assert.HasCount(17, tokens);
        Assert.AreEqual(JsonTokenKind.OpeningBrace, tokens[0].Kind);
        Assert.AreEqual(JsonTokenKind.ClosingBrace, tokens[16].Kind);
        Assert.AreEqual(JsonTokenKind.String, tokens[5].Kind);
        Assert.AreEqual(JsonTokenKind.Comma, tokens[12].Kind);
        Assert.AreEqual(JsonTokenKind.Colon, tokens[10].Kind);
        Assert.AreEqual("range", tokens[5].Value);
        Assert.AreEqual("2", tokens[15].Value);
    }
}