using Json.Parsing;
using Json.Document;

namespace Json.Tests.UnitTests;

/// <summary>
/// <Ensures token grammar is valid.
/// Ensures the correct <see cref="JsonValue"/> records are created with their correct values.
/// </summary>
[TestClass]
[TestCategory("JsonParser")]
public sealed class JsonParserTests
{
    [TestMethod]
    public void ParseStringTest()
    {
        var value = JsonParser.Parse([new JsonToken(JsonTokenKind.String, "wireframe")]);
        Assert.IsInstanceOfType<JsonValue.JsonString>(value);

        var actual = value as JsonValue.JsonString;
        Assert.IsNotNull(actual);

        Assert.AreEqual("wireframe", actual.Value);
    }

    [TestMethod]
    public void ParseNumberTest()
    {
        var value = JsonParser.Parse([new JsonToken(JsonTokenKind.Number, "2.5E+682")]);
        Assert.IsInstanceOfType<JsonValue.JsonNumber>(value);

        var actual = value as JsonValue.JsonNumber;
        Assert.IsNotNull(actual);

        Assert.AreEqual("2.5E+682", actual.Raw);
        Assert.IsTrue(actual.TryGetDouble(out _));
    }

    [TestMethod]
    public void ParseTrueTest()
    {
        var value = JsonParser.Parse([new JsonToken(JsonTokenKind.True, "true")]);
        Assert.IsInstanceOfType<JsonValue.JsonBoolean>(value);

        var actual = value as JsonValue.JsonBoolean;
        Assert.IsNotNull(actual);

        Assert.IsTrue(actual.Value);
    }

    [TestMethod]
    public void ParseFalseTest()
    {
        var value = JsonParser.Parse([new JsonToken(JsonTokenKind.False, "false")]);
        Assert.IsInstanceOfType<JsonValue.JsonBoolean>(value);

        var actual = value as JsonValue.JsonBoolean;
        Assert.IsNotNull(actual);

        Assert.IsFalse(actual.Value);
    }

    [TestMethod]
    public void ParseNullTest()
    {
        var value = JsonParser.Parse([new JsonToken(JsonTokenKind.Null, "null")]);
        Assert.IsInstanceOfType<JsonValue.JsonNull>(value);

        var actual = value as JsonValue.JsonNull;
        Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void ParseArrayTest()
    {
        var value = JsonParser.Parse([
           new JsonToken(JsonTokenKind.OpeningBracket, "["),
           new JsonToken(JsonTokenKind.String, "Apples"),
           new JsonToken(JsonTokenKind.Comma, ","),
           new JsonToken(JsonTokenKind.Number, "22"),
           new JsonToken(JsonTokenKind.ClosingBracket, "]"), 
        ]);
        Assert.IsInstanceOfType<JsonValue.JsonArray>(value);

        var actual = value as JsonValue.JsonArray;
        Assert.IsNotNull(actual);
        Assert.HasCount(2, actual.Items);
    }

    [TestMethod]
    public void ParseInvalidArrayTest()
    {
        Assert.Throws<FormatException>(() => JsonParser.Parse([
            new JsonToken(JsonTokenKind.OpeningBracket, "["),
            new JsonToken(JsonTokenKind.String, "Apples"),
            new JsonToken(JsonTokenKind.Comma, ","),
            new JsonToken(JsonTokenKind.Number, "22"),
        ]));

        Assert.Throws<FormatException>(() => JsonParser.Parse([
            new JsonToken(JsonTokenKind.OpeningBracket, "["),
            new JsonToken(JsonTokenKind.String, "Apples"),
            new JsonToken(JsonTokenKind.Comma, ","),
            new JsonToken(JsonTokenKind.ClosingBracket, "]"), 
        ]));
    }

    [TestMethod]
    public void ParseObjectTest()
    {
        var value = JsonParser.Parse([
            new JsonToken(JsonTokenKind.OpeningBrace, "{"),
            new JsonToken(JsonTokenKind.String, "health"),
            new JsonToken(JsonTokenKind.Colon, ":"),
            new JsonToken(JsonTokenKind.Number, "100"),
            new JsonToken(JsonTokenKind.Comma, ","),
            new JsonToken(JsonTokenKind.String, "ammo"),
            new JsonToken(JsonTokenKind.Colon, ":"),
            new JsonToken(JsonTokenKind.Number, "23"),
            new JsonToken(JsonTokenKind.ClosingBrace, "}"),
        ]);
        Assert.IsInstanceOfType<JsonValue.JsonObject>(value);

        var actual = value as JsonValue.JsonObject;
        Assert.IsNotNull(actual);
        Assert.HasCount(2, actual.Fields);
        Assert.AreEqual("100", ((JsonValue.JsonNumber)actual.Fields[new JsonValue.JsonString("health")]).Raw);
        Assert.AreEqual("23", ((JsonValue.JsonNumber)actual.Fields[new JsonValue.JsonString("ammo")]).Raw);
    }
}