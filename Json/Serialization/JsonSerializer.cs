using Json.Document;

namespace Json.Serialization;

/// <summary>
/// Turns C# value(s) into a JSON string.
/// <para>Serializes all public fields and properties.</para>
/// <para>Supported Types:</para>
/// <list type="bullet">
/// <item><see cref="int"/></item>
/// <item><see cref="long"/></item>
/// <item><see cref="double"/></item>
/// <item><see cref="decimal"/></item>
/// <item><see cref="string"/></item>
/// <item><see cref="bool"/></item>
/// <item><see langword="null"/></item>
/// <item><see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> of <see cref="string"/> and <see cref="object"/>?</item>
/// <item><see cref="System.Collections.IEnumerable"/></item>
/// </list>
/// </summary>
public class JsonSerializer
{
    /// <summary>
    /// Throws <see cref="ArgumentException"/> on serialize failure.
    /// <para>Throws <see cref="FormatException"/> on write failure.</para>
    /// </summary>
    public static string Serialize(object? value) => JsonDocument.Write(SerializeValue(value));

    private static JsonValue SerializeValue(object? value)
    {
        return value switch
        {
            int actual => new JsonValue.JsonNumber(actual.ToString()),
            long actual => new JsonValue.JsonNumber(actual.ToString()),
            double actual => new JsonValue.JsonNumber(actual.ToString()),
            decimal actual => new JsonValue.JsonNumber(actual.ToString()),
            string actual => new JsonValue.JsonString(actual),
            bool actual => new JsonValue.JsonBoolean(actual),
            null => new JsonValue.JsonNull(),
            IEnumerable<KeyValuePair<string, object?>> actual => SerializeObject(actual),
            System.Collections.IEnumerable actual => SerializeList(actual),
            _ => SerializeObject(value)
        };
    }

    private static JsonValue.JsonArray SerializeList(System.Collections.IEnumerable value)
    {
        List<JsonValue> items = [];

        foreach (var item in value)
            items.Add(SerializeValue(item));
        
        return new JsonValue.JsonArray([.. items]);
    }

    private static JsonValue.JsonObject SerializeObject(IEnumerable<KeyValuePair<string, object?>> value)
    {
        List<KeyValuePair<JsonValue.JsonString, JsonValue>> fields = [];

        foreach (var pair in value)
        {
            fields.Add(
                new(
                    new JsonValue.JsonString(pair.Key),
                    SerializeValue(pair.Value)
                )
            );
        }

        return new JsonValue.JsonObject([.. fields]);
    }

    private static JsonValue.JsonObject SerializeObject(object value)
    {
        var valueType = value.GetType();
        var valueFields = valueType.GetFields();
        var valueProperties = valueType.GetProperties();

        if (valueFields.Length == 0 && valueProperties.Length == 0)
            throw new ArgumentException($"Unsupported Type: {value.GetType()}. Has no public fields or properties");

        List<KeyValuePair<JsonValue.JsonString, JsonValue>> fields = [];

        foreach (var valueField in valueFields)
        {
            fields.Add(
                new(
                    new JsonValue.JsonString(valueField.Name),
                    SerializeValue(valueField.GetValue(value))
                )
            );
        }

        foreach (var valueProperty in valueProperties)
        {
            fields.Add(
                new(
                    new JsonValue.JsonString(valueProperty.Name),
                    SerializeValue(valueProperty.GetValue(value))
                )
            );
        }

        return new JsonValue.JsonObject([.. fields]);
    }
}