using Json.Parsing;

namespace Json.Document;

public static class JsonDocument
{
    public static JsonValue Parse(string source) => JsonParser.Parse(JsonLexer.Tokenise(source));

    public static string Write(JsonValue json)
    {
        return json switch
        {
            JsonValue.JsonString => $"\"{((JsonValue.JsonString)json).Value}\"",
            JsonValue.JsonNumber => ((JsonValue.JsonNumber)json).Raw,
            JsonValue.JsonBoolean => ((JsonValue.JsonBoolean)json).Value ? "true" : "false",
            JsonValue.JsonNull => "null",
            JsonValue.JsonArray => WriteArray((JsonValue.JsonArray)json),
            // JsonValue.JsonObject => WriteObject((JsonValue.JsonObject)json),
            _ => throw new FormatException("Invalid JsonValue")
        };
    }

    private static string WriteArray(JsonValue.JsonArray jsonArray)
    {
        string result = "[";

        bool firstItem = true;
        foreach (var item in jsonArray.Items)
        {
            if (!firstItem)
                result += ", ";
            else
                firstItem = false;

            result += Write(item);
        }

        return result + "]";
    }
}