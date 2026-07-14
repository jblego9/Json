namespace Json;

public class JsonParser
{
    public static JsonValue Parse(List<JsonToken> tokens)
    {
        var parser = new JsonParser(tokens);
        return parser.InternalParse();
    }

    private JsonParser(List<JsonToken> tokens) => this.tokens = tokens;

    private readonly List<JsonToken> tokens;
    private int position = 0;
    private JsonToken Consume()  => tokens[position++];

    private JsonValue InternalParse()
    {
        JsonToken token = Consume();
        return token.Kind switch
        {
            JsonTokenKind.String => new JsonValue.JsonString(token.Value),
            JsonTokenKind.Number => new JsonValue.JsonNumber(token.Value),
            JsonTokenKind.True => new JsonValue.JsonBoolean(true),
            JsonTokenKind.False => new JsonValue.JsonBoolean(false),
            JsonTokenKind.Null => new JsonValue.JsonNull(),
            // JsonTokenKind.OpeningBracket => ParseArray(),
            _ => throw new FormatException($"Unexpected token '{token}' at position: {position}"),
        };
    }

    private JsonValue ParseArray()
    {
        // Opening bracket has already been consumed.

        throw new NotImplementedException();
    }
}