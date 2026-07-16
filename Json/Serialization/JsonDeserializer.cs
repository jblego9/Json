using Json.Document;

namespace Json.Serialization;

public class JsonDeserializer
{
    public static T Deserialize<T>(string json)
    {
        return DeserializeValue<T>(json);
    }

    private static T DeserializeValue<T>(string json)
    {
        var targetType = typeof(T);
        var value = JsonDocument.Parse(json);

        if (value is JsonValue.JsonNull)
            return DeserializeNull<T>(targetType);

        if (targetType == typeof(string))
            return (T)Convert.ChangeType(((JsonValue.JsonString)value).Value, targetType);
        else if (targetType == typeof(int))
            return (T)Convert.ChangeType(((JsonValue.JsonNumber)value).AsInt32(), targetType);
        else if (targetType == typeof(long))
            return (T)Convert.ChangeType(((JsonValue.JsonNumber)value).AsInt64(), targetType);
        else if (targetType == typeof(double))
            return (T)Convert.ChangeType(((JsonValue.JsonNumber)value).AsDouble(), targetType);
        else if (targetType == typeof(decimal))
            return (T)Convert.ChangeType(((JsonValue.JsonNumber)value).AsDecimal(), targetType);
        else if (targetType == typeof(bool))
            return (T)Convert.ChangeType(((JsonValue.JsonBoolean)value).Value, targetType);
        else
            throw new ArgumentException($"Cannot deserialize JSON to target type '{targetType}'");
    }

    private static T DeserializeNull<T>(Type targetType)
    {
        if (!targetType.IsValueType)
            return default!;
        
        if (Nullable.GetUnderlyingType(targetType) != null)
            return default!;
        
        throw new ArgumentException($"Cannot convert JSON null to non-nullable type '{targetType}'");
    }
}