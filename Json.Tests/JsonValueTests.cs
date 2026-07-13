using System.Collections.Immutable;

namespace Json.Tests;

/// <summary>
/// <para>Ensures equality based on values and not object references.</para>
/// <para>Ensures item order sensitivity for <see cref="JsonValue.JsonArray"/> and field order insensitivity for <see cref="JsonValue.JsonObject"/></para>
/// <para>Ensures number equality regardless of asXXXX or tryGetXXXX methods for <see cref="JsonValue.JsonNumber"/></para>
/// <para>Ensures attempting to get a number via asXXXX or tryGetXXXX as the wrong data type fails.</para>
/// </summary>
[TestClass]
[TestCategory("JsonValue")]
public sealed class JsonValueTests
{
    [TestMethod]
    public void JsonStringEqualityTest()
    {
        var a = new JsonValue.JsonString("x");
        var b = new JsonValue.JsonString("x");

        // Equality should be based on string value and not object reference.
        Assert.AreEqual(a, b);
    }

    [TestMethod]
    public void JsonNumberDoubleEqualityTest()
    {
        var a = new JsonValue.JsonNumber("2.5");
        var b = new JsonValue.JsonNumber("2.5");

        // Equality should be based on internal string value and not object reference.
        Assert.AreEqual(a, b);

        Assert.IsTrue(a.TryGetDouble(out var tryGetDouble));
        
        // Should be equal regardless of asXXXX or tryGetXXXX.
        Assert.AreEqual(a.AsDouble(), tryGetDouble);
    }

    [TestMethod]
    public void JsonNumberDecimalEqualityTest()
    {
        var a = new JsonValue.JsonNumber("2.5");
        var b = new JsonValue.JsonNumber("2.5");

        // Equality should be based on internal string value and not object reference.
        Assert.AreEqual(a, b);

        Assert.IsTrue(a.TryGetDecimal(out var tryGetDecimal));
        
        // Should be equal regardless of asXXXX or tryGetXXXX.
        Assert.AreEqual(a.AsDecimal(), tryGetDecimal);
    }

    [TestMethod]
    public void JsonNumberInt32EqualityTest()
    {
        var a = new JsonValue.JsonNumber("25");
        var b = new JsonValue.JsonNumber("25");

        // Equality should be based on internal string value and not object reference.
        Assert.AreEqual(a, b);

        Assert.IsTrue(a.TryGetInt32(out var tryGetInt32));
        
        // Should be equal regardless of asXXXX or tryGetXXXX.
        Assert.AreEqual(a.AsInt32(), tryGetInt32);
    }

    [TestMethod]
    public void JsonNumberInt64EqualityTest()
    {
        var a = new JsonValue.JsonNumber("25");
        var b = new JsonValue.JsonNumber("25");

        // Equality should be based on internal string value and not object reference.
        Assert.AreEqual(a, b);

        Assert.IsTrue(a.TryGetInt64(out var tryGetInt64));
        
        // Should be equal regardless of asXXXX or tryGetXXXX.
        Assert.AreEqual(a.AsInt64(), tryGetInt64);
    }

    [TestMethod]
    public void InvalidAsNumberTest()
    {
        Assert.Throws<FormatException>(() => new JsonValue.JsonNumber("123abc").AsDouble());
        Assert.Throws<FormatException>(() => new JsonValue.JsonNumber("123abc").AsDecimal());
        Assert.Throws<FormatException>(() => new JsonValue.JsonNumber("2.5").AsInt32());
        Assert.Throws<FormatException>(() => new JsonValue.JsonNumber("2.5").AsInt64());
    }

    [TestMethod]
    public void InvalidTryGetNumberTest()
    {
        Assert.IsFalse(new JsonValue.JsonNumber("123abc").TryGetDouble(out _));
        Assert.IsFalse(new JsonValue.JsonNumber("123abc").TryGetDecimal(out _));
        Assert.IsFalse(new JsonValue.JsonNumber("2.5").TryGetInt32(out _));
        Assert.IsFalse(new JsonValue.JsonNumber("2.5").TryGetInt64(out _));
    }

    [TestMethod]
    public void JsonObjectEqualityTest()
    {
        var a = new JsonValue.JsonObject(
            ImmutableDictionary.CreateRange<JsonValue.JsonString, JsonValue>([
                new(
                    new JsonValue.JsonString("health"),
                    new JsonValue.JsonNumber("100")
                ),
                new(
                    new JsonValue.JsonString("ammo"),
                    new JsonValue.JsonNumber("22")
                )
            ])
        );

        var b = new JsonValue.JsonObject(
            ImmutableDictionary.CreateRange<JsonValue.JsonString, JsonValue>([
                new(
                    new JsonValue.JsonString("health"),
                    new JsonValue.JsonNumber("100")
                ),
                new(
                    new JsonValue.JsonString("ammo"),
                    new JsonValue.JsonNumber("22")
                )
            ])
        );

        // Ensure equality is based on fields keys and values, and not object reference.
        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

        var c = new JsonValue.JsonObject(
            ImmutableDictionary.CreateRange<JsonValue.JsonString, JsonValue>([
                new(
                    new JsonValue.JsonString("ammo"),
                    new JsonValue.JsonNumber("22")
                ),
                new(
                    new JsonValue.JsonString("health"),
                    new JsonValue.JsonNumber("100")
                )
            ])
        );

        // Field order should not impact equality.
        Assert.AreEqual(a, c);
        Assert.AreEqual(a.GetHashCode(), c.GetHashCode());
    }

    [TestMethod]
    public void JsonArrayEqualityTest()
    {
        var a = new JsonValue.JsonArray([
            new JsonValue.JsonNumber("1"),
            new JsonValue.JsonNumber("2"),
            new JsonValue.JsonNumber("3"),
        ]);
        var b = new JsonValue.JsonArray([
            new JsonValue.JsonNumber("1"),
            new JsonValue.JsonNumber("2"),
            new JsonValue.JsonNumber("3"),
        ]);

        // Equality should be based on the sequence of items and not object reference.
        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

        var c = new JsonValue.JsonArray([
            new JsonValue.JsonNumber("1"),
            new JsonValue.JsonNumber("3"),
            new JsonValue.JsonNumber("2"),
        ]);

        // Item sequence should impact equality.
        Assert.AreNotEqual(b, c);
        Assert.AreNotEqual(b.GetHashCode(), c.GetHashCode());
    }

    [TestMethod]
    public void JsonBooleanEqualityTest()
    {
        var a = new JsonValue.JsonBoolean(true);
        var b = new JsonValue.JsonBoolean(true);

        // Equality should be based on bool value and not object reference.
        Assert.AreEqual(a, b);
    }

    [TestMethod]
    public void JsonNullEqualityTest()
    {
        var a = new JsonValue.JsonNull();
        var b = new JsonValue.JsonNull();

        // These should never not be equal.
        Assert.AreEqual(a, b);
    }
}
