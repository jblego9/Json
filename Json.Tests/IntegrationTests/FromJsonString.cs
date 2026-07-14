namespace Json.Tests.IntegrationTests;

[TestClass]
[TestCategory("FromJsonStringTests")]
public sealed class FromJsonStringTests
{
    [TestMethod]
    public void JsonArrayFromStringTest()
    {
        Document.JsonValue parsed = Document.JsonDocument.Parse(" [123, \"apples\", true, false, null, [1, 2, 3], { \"reloading\": false} ]");
        Assert.IsInstanceOfType<Document.JsonValue.JsonArray>(parsed);

        var actual = (Document.JsonValue.JsonArray)parsed;
        Assert.IsNotNull(actual);
        Assert.HasCount(7, actual.Items);

        var subArray = (Document.JsonValue.JsonArray)actual.Items[5];
        Assert.IsNotNull(subArray);
        Assert.HasCount(3, subArray.Items);

        var innerObject = (Document.JsonValue.JsonObject)actual.Items[6];
        Assert.IsNotNull(innerObject);
        Assert.HasCount(1, innerObject.Fields);
    }

    [TestMethod]
    public void JsonObjectFromStringTest()
    {
        Document.JsonValue parsed = Document.JsonDocument.Parse("""
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
        Assert.IsInstanceOfType<Document.JsonValue.JsonObject>(parsed);

        var actual = (Document.JsonValue.JsonObject)parsed;
        Assert.IsNotNull(actual);
        Assert.HasCount(5, actual.Fields);

        var subObject = (Document.JsonValue.JsonObject)actual.Fields[new Document.JsonValue.JsonString("randomObject")];
        Assert.IsNotNull(subObject);
        Assert.HasCount(2, subObject.Fields);
    }
}