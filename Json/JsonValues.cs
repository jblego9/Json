using System.Collections.Immutable;
using System.Globalization;

namespace Json;

/// <summary>
/// Represents a JSON value. This is a closed hierarchy, only allowing for subtypes nested within this type.
/// The constructor is marked private to prevent external types from inheriting from <see cref="JsonValue"/>.
/// </summary>
public abstract record JsonValue
{
    private JsonValue() {}

    public sealed record JsonString(string Value) : JsonValue;

    /// <summary>
    /// Represents a JSON number. Internally stores a string which intended to be parsed as the
    /// desired numeric type. Equality tests the internal string and not a numeric value.
    /// <para>The TryGetXXXX methods return a bool indicating success or failure.</para>
    /// <para>The AsXXXX methods internally use the TryGetXXXX methods and throw on failure.</para>
    /// </summary>
    public sealed record JsonNumber(string Raw) : JsonValue
    {
        public double AsDouble() =>
            TryGetDouble(out var value) ? value : throw new FormatException($"'{Raw}' is not a valid double.");

        public decimal AsDecimal() =>
            TryGetDecimal(out var value) ? value : throw new FormatException($"'{Raw}' is not a valid decimal.");

        public int AsInt32() => 
            TryGetInt32(out var value) ? value : throw new FormatException($"'{Raw}' is not a valid Int32.");

        public long AsInt64() =>
            TryGetInt64(out var value) ? value : throw new FormatException($"'{Raw}' is not a valid Int64.");

        public bool TryGetDouble(out double value) => double.TryParse(Raw, CultureInfo.InvariantCulture, out value);
        public bool TryGetDecimal(out decimal value) => decimal.TryParse(Raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        public bool TryGetInt32(out int value) => int.TryParse(Raw, CultureInfo.InvariantCulture, out value);
        public bool TryGetInt64(out long value) => long.TryParse(Raw, CultureInfo.InvariantCulture, out value);

        public override string ToString() => Raw;
    }

    public sealed record JsonObject
    {
        public ImmutableDictionary<JsonString, JsonValue> Fields { get; }

        // Protects against null dictionary.
        public JsonObject(ImmutableDictionary<JsonString, JsonValue> fields) => Fields = fields is null ? [] : fields;

        // Ensure it is based on field's keys and values, and not field order.
        public bool Equals(JsonObject? other)
        {
            if (other is null || Fields.Count != other.Fields.Count)
                return false;

            foreach (var field in Fields)
            {
                if (!other.Fields.TryGetValue(field.Key, out var value) || value != field.Value)
                    return false;
            }

            return true;
        }

        // Ensure it is based on field's keys and values, and not field order.
        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var field in Fields)
            {
                hash.Add(field.Key);
                hash.Add(field.Value);
            }
            return hash.ToHashCode();
        }
    }

    public sealed record JsonArray
    {
        public ImmutableArray<JsonValue> Items { get; }

        // Protects against default/uninitialised arary.
        public JsonArray(ImmutableArray<JsonValue> items) => Items = items.IsDefault ? [] : items;

        // Ensure it is based on the array's sequence of items.
        public bool Equals(JsonArray? other) => other is not null && Items.SequenceEqual(other.Items);
        
        // Ensure it is based on the array's sequence of items.
        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var item in Items) hash.Add(item);
            return hash.ToHashCode();
        }
    }

    public sealed record JsonBoolean(bool Value) : JsonValue;

    public sealed record JsonNull : JsonValue;
}
