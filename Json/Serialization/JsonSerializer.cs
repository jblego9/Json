using Json.Document;

namespace Json.Serialization;

public class JsonSerializer
{
    public static string Serialize(object? value) => JsonDocument.Write(SerializeValue(value));

    private static JsonValue SerializeValue(object? value)
    {
        return value switch
        {
            int => new JsonValue.JsonNumber(((int)value).ToString()),
            long => new JsonValue.JsonNumber(((long)value).ToString()),
            double => new JsonValue.JsonNumber(((double)value).ToString()),
            decimal => new JsonValue.JsonNumber(((decimal)value).ToString()),
            string => new JsonValue.JsonString((string)value),
            bool => new JsonValue.JsonBoolean((bool)value),
            null => new JsonValue.JsonNull(),
            List<object?> => SerializeList((List<object?>)value),
            List<KeyValuePair<string, object?>> => SerializeObject((List<KeyValuePair<string, object?>>)value),
            _ => SerializeObject(value)
        };
    }

    private static JsonValue.JsonArray SerializeList(List<object?> value)
    {
        List<JsonValue> items = [];

        foreach (var item in value)
            items.Add(SerializeValue(item));
        
        return new JsonValue.JsonArray([.. items]);
    }

    private static JsonValue.JsonObject SerializeObject(List<KeyValuePair<string, object?>> value)
    {
        List<KeyValuePair<JsonValue.JsonString, JsonValue>> fields = [];

        foreach (var field in value)
        {
            fields.Add(
                new(
                    new JsonValue.JsonString(field.Key),
                    SerializeValue(field.Value)
                )
            );
        }

        return new JsonValue.JsonObject([.. fields]);
    }

    private static JsonValue.JsonObject SerializeObject(object value)
    {
        throw new ArgumentException($"Unsupported Type: {value.GetType()}");
    }
}