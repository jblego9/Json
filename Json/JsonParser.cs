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
    private bool IsFinished() => position >= tokens.Count;
    private JsonToken Consume()  => tokens[position++];
    private JsonToken At() => tokens[position];

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
            JsonTokenKind.OpeningBracket => ParseArray(),
            _ => throw new FormatException($"Unexpected token '{token}' at position: {position}"),
        };
    }

    private JsonValue.JsonArray ParseArray()
    {
        // Opening bracket has already been consumed.
        List<JsonValue> items = [];

        bool gotComma = false;
        while (!IsFinished())
        {
            if (At().Kind == JsonTokenKind.ClosingBracket)
            {
                if (gotComma)
                    throw new FormatException($"Expected value after ',' at position: {position}");

                Consume();
                return new JsonValue.JsonArray([.. items]);
            }

            items.Add(InternalParse());
            gotComma = false;

            // InternalParse advances position, meaning checking for a comma can throw.
            if (IsFinished())
                break;

            if (At().Kind == JsonTokenKind.Comma)
            {
                gotComma = true;
                Consume();
            }
        }

        if (gotComma)
            throw new FormatException($"Expected value after ',' at position: {position}");

        throw new FormatException($"Expected ']' at end of array, at position: {position}");
    }
}