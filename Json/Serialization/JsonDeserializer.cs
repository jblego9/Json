using Json.Document;

namespace Json.Serialization;

/// <summary>
/// Turns a JSON string into C# value(s).
/// <para>Deserializes into all public fields and properties.</para>
/// <para>Supported Types:</para>
/// <list type="bullet">
/// <item><see cref="int"/></item>
/// <item><see cref="long"/></item>
/// <item><see cref="double"/></item>
/// <item><see cref="decimal"/></item>
/// <item><see cref="string"/></item>
/// <item><see cref="bool"/></item>
/// <item><see langword="null"/></item>
/// <item><see cref="object"/> (type inferred from the JSON value)</item>
/// <item>Arrays and <see cref="List{T}"/></item>
/// <item><see cref="Dictionary{TKey, TValue}"/> of <see cref="string"/> and TValue</item>
/// <item>Plain classes/structs with public fields/properties. Must have a parameterless constructor.</item>
/// </list>
/// </summary>
public class JsonDeserializer
{
    /// <summary>
    /// Throws <see cref="ArgumentException"/> on deserialize failure.
    /// <para>Throws <see cref="FormatException"/> on invalid number format.</para>
    /// </summary>
    public static T Deserialize<T>(string json)
    {
        var value = JsonDocument.Parse(json);
        return (T)DeserializeValue(typeof(T), value)!;
    }

    private static object? DeserializeValue(Type targetType, JsonValue value)
    {
        if (targetType == typeof(object))
            return DeserializeUntyped(value);

        return value switch
        {
            JsonValue.JsonString actual when targetType == typeof(string) => actual.Value,
            JsonValue.JsonNumber actual when targetType == typeof(int) => actual.AsInt32(),
            JsonValue.JsonNumber actual when targetType == typeof(long) => actual.AsInt64(),
            JsonValue.JsonNumber actual when targetType == typeof(double) => actual.AsDouble(),
            JsonValue.JsonNumber actual when targetType == typeof(decimal) => actual.AsDecimal(),
            JsonValue.JsonBoolean actual when targetType == typeof(bool) => actual.Value,
            JsonValue.JsonArray actual => DeserializeArray(targetType, actual),
            JsonValue.JsonObject actual => DeserializeObject(targetType, actual),
            JsonValue.JsonNull => DeserializeNull(targetType),
            _ => throw new ArgumentException($"Unsupported type: {targetType}. Cannot deserialize JSON into it")
        };
    }

    private static object? DeserializeUntyped(JsonValue value) => value switch
    {
        JsonValue.JsonNull => null,
        JsonValue.JsonString actual => actual.Value,
        JsonValue.JsonBoolean actual => actual.Value,
        JsonValue.JsonNumber actual => DeserializeUntypedNumber(actual),
        JsonValue.JsonArray actual => DeserializeArray(typeof(List<object>), actual),
        JsonValue.JsonObject actual => DeserializeObject(typeof(Dictionary<string, object?>), actual),
        _ => throw new ArgumentException($"Unsupported JSON value type: {value.GetType()}"),
    };

    private static object DeserializeUntypedNumber(JsonValue.JsonNumber number)
    {
        if (number.TryGetInt32(out var i)) return i;
        if (number.TryGetInt64(out var l)) return l;
        if (number.TryGetDecimal(out var d)) return d;
        if (number.TryGetDouble(out var dbl)) return dbl;

        throw new FormatException($"'{number.Raw}' is not a valid number");
    }

    private static object? DeserializeNull(Type targetType)
    {
        if (!targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null)
            return null;

        throw new ArgumentException($"Cannot convert JSON null into non-nullable type: {targetType}");
    }

    private static object DeserializeArray(Type targetType, JsonValue.JsonArray jsonArray)
    {
        var elementType = targetType.IsArray
            ? targetType.GetElementType()!
            : targetType.GetGenericArguments().ElementAtOrDefault(0)
                ?? throw new ArgumentException($"Unsupported type: {targetType}. Expected an array or List<T>");
        
        var items = jsonArray.Items.Select(item => DeserializeValue(elementType, item)).ToList();

        if (targetType.IsArray)
        {
            var array = Array.CreateInstance(elementType, items.Count);
            for (var i = 0; i < items.Count; i++)
                array.SetValue(items[i], i);
            return array;
        }

        var list = (System.Collections.IList)Activator.CreateInstance(targetType)!;
        foreach (var item in items)
            list.Add(item);

        return list;
    }

    private static object DeserializeObject(Type targetType, JsonValue.JsonObject value)
    {
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var keyType = targetType.GetGenericArguments()[0];
            var valueType = targetType.GetGenericArguments()[1];

            if (keyType != typeof(string))
                throw new ArgumentException($"Unsupported Type: {targetType}. Only Dictionary<string, TValue> is supported");

            var dict = (System.Collections.IDictionary)Activator.CreateInstance(targetType)!;
            foreach (var (fieldName, fieldValue) in value.Fields)
                dict[fieldName.Value] = DeserializeValue(valueType, fieldValue);

            return dict;
        }

        var instance = Activator.CreateInstance(targetType)
            ?? throw new ArgumentException($"Unsupported Type: {targetType}. Could not create an instance of it");

        foreach (var (fieldName, fieldValue) in value.Fields)
        {
            var field = targetType.GetField(fieldName.Value);
            if (field != null)
            {
                field.SetValue(instance, DeserializeValue(field.FieldType, fieldValue));
                continue;
            }

            var property = targetType.GetProperty(fieldName.Value);
            if (property != null && property.CanWrite)
                property.SetValue(instance, DeserializeValue(property.PropertyType, fieldValue));

            // unknown/read-only members are ignored
        }

        return instance;
    }
}