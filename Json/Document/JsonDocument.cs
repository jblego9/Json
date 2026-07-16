using Json.Parsing;

namespace Json.Document;

/// <summary>
/// Can parse a JSON string and create a <see cref="JsonValue"/> tree, and can write a JSON string from a <see cref="JsonValue"/> tree.
/// <para>The caller is expected to read from or create the <see cref="JsonValue"/> tree themselves when parsing or writing respectively.</para>
/// </summary>
public static class JsonDocument
{
    /// <summary>
    /// Throws <see cref="FormatException"/> on failure.
    /// </summary>
    public static JsonValue Parse(string source) => JsonParser.Parse(JsonLexer.Tokenise(source));

    /// <summary>
    /// Throws <see cref="FormatException"/> on failure.
    /// </summary>
    public static string Write(JsonValue json)
    {
        return json switch
        {
            JsonValue.JsonString => WriteString((JsonValue.JsonString)json),
            JsonValue.JsonNumber => ((JsonValue.JsonNumber)json).Raw,
            JsonValue.JsonBoolean => ((JsonValue.JsonBoolean)json).Value ? "true" : "false",
            JsonValue.JsonNull => "null",
            JsonValue.JsonArray => WriteArray((JsonValue.JsonArray)json),
            JsonValue.JsonObject => WriteObject((JsonValue.JsonObject)json),
            _ => throw new FormatException("Invalid JsonValue")
        };
    }

    private static string WriteString(JsonValue.JsonString jsonString)
    {
        string result = "\"";

        for (int i = 0; i < jsonString.Value.Length; i++)
        {
            char c = jsonString.Value[i];
            if (c == '"')
            {
                if (i - 1 < 0)
                    throw new FormatException("Unescaped '\"' within string");

                if (jsonString.Value[i - 1] != '\\')
                    throw new FormatException("Unescaped '\"' within string");
            }

            result += c;
        }

        return result + "\"";
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

    private static string WriteObject(JsonValue.JsonObject jsonObject)
    {
        string result = "{";

        bool firstField = true;
        foreach (var field in jsonObject.Fields)
        {
            if (!firstField)
                result += ", ";
            else
                firstField = false;
            
            result += Write(field.Key) + ": " + Write(field.Value);
        }

        return result + "}";
    }
}