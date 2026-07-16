using System.Collections.Immutable;
using Json.Serialization;

namespace Json.Tests.IntegrationTests;

[TestClass]
[TestCategory("Serialize")]
public sealed class SerializeTests
{
    [TestMethod]
    public void SerializeSingleValues()
    {
        Assert.AreEqual("100", JsonSerializer.Serialize(100));
        Assert.AreEqual("9999999999", JsonSerializer.Serialize(9999999999));
        Assert.AreEqual("122.5", JsonSerializer.Serialize(122.5));
        Assert.AreEqual("-2.5E+100", JsonSerializer.Serialize(-2.5E+100));
        Assert.AreEqual("\"hello\"", JsonSerializer.Serialize("hello"));
        Assert.AreEqual("true", JsonSerializer.Serialize(true));
        Assert.AreEqual("false", JsonSerializer.Serialize(false));
        Assert.AreEqual("null", JsonSerializer.Serialize(null));
    }

    [TestMethod]
    public void SerializeList()
    {
        List<object?> list = [1, true, false, null, 100, -2.5E+100, "hello"];
        Assert.AreEqual("[1, true, false, null, 100, -2.5E+100, \"hello\"]", JsonSerializer.Serialize(list));
    }

    [TestMethod]
    public void SerializeDictionary()
    {
        var dictionary = new OrderedDictionary<string, object?> {
            {"ammo", 100},
            {"a", true},
            {"b", false},
            {"null", null},
            {"int", 100},
            {"d", -2.5E+100},
            {"text", "hello"}
        };
        Assert.AreEqual(
            "{\"ammo\": 100, \"a\": true, \"b\": false, \"null\": null, \"int\": 100, \"d\": -2.5E+100, \"text\": \"hello\"}",
            JsonSerializer.Serialize(dictionary.ToList())
        );
    }
}