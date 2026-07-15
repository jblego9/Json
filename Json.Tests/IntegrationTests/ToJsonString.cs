using Json.Document;

namespace Json.Tests.IntegrationTests;

[TestClass]
[TestCategory("ToJsonString")]
public sealed class ToJsonStringTests
{
    [TestMethod]
    public void JsonArrayToStringTest()
    {
        string fromJson = JsonDocument.Write(
            new JsonValue.JsonArray([
                new JsonValue.JsonString("apples"),
                new JsonValue.JsonNumber("30"),
                new JsonValue.JsonBoolean(true),
                new JsonValue.JsonBoolean(false),
                new JsonValue.JsonNull(),
                new JsonValue.JsonArray([
                    new JsonValue.JsonNumber("1"),
                    new JsonValue.JsonNumber("2"),
                    new JsonValue.JsonNumber("3")
                ])
            ])
        );
        Assert.AreEqual("[\"apples\", 30, true, false, null, [1, 2, 3]]", fromJson);

        JsonValue backToJson = JsonDocument.Parse(fromJson);
        Assert.IsInstanceOfType<JsonValue.JsonArray>(backToJson);

        string fromJsonAgain = JsonDocument.Write(backToJson);
        Assert.AreEqual(fromJson, fromJsonAgain);
    }
}