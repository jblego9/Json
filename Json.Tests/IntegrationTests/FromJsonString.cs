using Json.Document;

namespace Json.Tests.IntegrationTests;

[TestClass]
[TestCategory("FromJsonString")]
public sealed class FromJsonStringTests
{
    [TestMethod]
    public void JsonArrayFromStringTest()
    {
        JsonValue parsed = JsonDocument.Parse(" [123, \"apples\", true, false, null, [1, 2, 3], { \"reloading\": false} ]");
        Assert.IsInstanceOfType<JsonValue.JsonArray>(parsed);

        var actual = (JsonValue.JsonArray)parsed;
        Assert.IsNotNull(actual);
        Assert.HasCount(7, actual.Items);

        var subArray = (JsonValue.JsonArray)actual.Items[5];
        Assert.IsNotNull(subArray);
        Assert.HasCount(3, subArray.Items);

        var innerObject = (JsonValue.JsonObject)actual.Items[6];
        Assert.IsNotNull(innerObject);
        Assert.HasCount(1, innerObject.Fields);
    }

    [TestMethod]
    public void JsonObjectFromStringTest()
    {
        JsonValue parsed = JsonDocument.Parse("""
                {
                    "damage": 10,
                    "headshotDamage": 20,
                    "fireRate": 0.5,
                    "range": 1000,
                    "randomObject": {
                        "a": true,
                        "b": false
                    }
                }
            """
        );
        Assert.IsInstanceOfType<JsonValue.JsonObject>(parsed);

        var actual = (JsonValue.JsonObject)parsed;
        Assert.IsNotNull(actual);
        Assert.HasCount(5, actual.Fields);

        var subObject = (JsonValue.JsonObject)actual.Fields[new JsonValue.JsonString("randomObject")];
        Assert.IsNotNull(subObject);
        Assert.HasCount(2, subObject.Fields);
    }
}