using System.Collections.Immutable;

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
                ]),
                new JsonValue.JsonObject(
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
                )
            ])
        );
        Assert.AreEqual("[\"apples\", 30, true, false, null, [1, 2, 3], {\"health\": 100, \"ammo\": 22}]", fromJson);

        JsonValue backToJson = JsonDocument.Parse(fromJson);
        Assert.IsInstanceOfType<JsonValue.JsonArray>(backToJson);

        string fromJsonAgain = JsonDocument.Write(backToJson);
        Assert.AreEqual(fromJson, fromJsonAgain);
    }

    [TestMethod]
    public void JsonObjectToStringTest()
    {
        string fromJson = JsonDocument.Write(
            new JsonValue.JsonObject(
                ImmutableDictionary.CreateRange<JsonValue.JsonString, JsonValue>([
                    new(
                        new JsonValue.JsonString("damage"),
                        new JsonValue.JsonNumber("10")
                    ),
                    new(
                        new JsonValue.JsonString("headshotDamage"),
                        new JsonValue.JsonNumber("20")
                    ),
                    new(
                        new JsonValue.JsonString("randomArray"),
                        new JsonValue.JsonArray([
                            new JsonValue.JsonBoolean(true),
                            new JsonValue.JsonBoolean(false),
                            new JsonValue.JsonNull()
                        ])
                    ),
                    new(
                        new JsonValue.JsonString("randomObject"),
                        new JsonValue.JsonObject(
                            ImmutableDictionary.CreateRange<JsonValue.JsonString, JsonValue>([
                                new(
                                    new JsonValue.JsonString("a"),
                                    new JsonValue.JsonString("b")
                                )
                            ])
                        )
                    )
                ])
            )
        );
        Assert.AreEqual("{\"damage\": 10, \"headshotDamage\": 20, \"randomArray\": [true, false, null], \"randomObject\": {\"a\": \"b\"}}", fromJson);
    }
}