namespace Json.Parsing;

public class JsonParser
{
    public static Document.JsonValue Parse(List<JsonToken> tokens)
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

    private Document.JsonValue InternalParse()
    {
        JsonToken token = Consume();
        return token.Kind switch
        {
            JsonTokenKind.String => new Document.JsonValue.JsonString(token.Value),
            JsonTokenKind.Number => new Document.JsonValue.JsonNumber(token.Value),
            JsonTokenKind.True => new Document.JsonValue.JsonBoolean(true),
            JsonTokenKind.False => new Document.JsonValue.JsonBoolean(false),
            JsonTokenKind.Null => new Document.JsonValue.JsonNull(),
            JsonTokenKind.OpeningBracket => ParseArray(),
            JsonTokenKind.OpeningBrace => ParseObject(),
            _ => throw new FormatException($"Unexpected token '{token}' at position: {position}"),
        };
    }

    private Document.JsonValue.JsonArray ParseArray()
    {
        // Opening bracket has already been consumed.
        List<Document.JsonValue> items = [];

        bool gotComma = false;
        while (!IsFinished())
        {
            if (At().Kind == JsonTokenKind.ClosingBracket)
            {
                if (gotComma)
                    throw new FormatException($"Expected value after ',' at position: {position}");

                Consume();
                return new Document.JsonValue.JsonArray([.. items]);
            }

            items.Add(InternalParse());
            gotComma = false;

            if (IsFinished())
                break;

            if (At().Kind == JsonTokenKind.Comma)
            {
                gotComma = true;
                Consume();
            }
        }

        if (gotComma)
            throw new FormatException($"Expected a value after ',' at position: {position}");

        throw new FormatException($"Expected ']' at end of array, at position: {position}");
    }

    private Document.JsonValue.JsonObject ParseObject()
    {
        // Opening brace has already been consumed.
        Dictionary<Document.JsonValue.JsonString, Document.JsonValue> fields = [];
        
        bool gotComma = false;
        while (!IsFinished())
        {
            if (At().Kind == JsonTokenKind.ClosingBrace)
            {
                if (gotComma)
                    throw new FormatException($"Expected field name after ',' at position: {position}");

                Consume();
                return new Document.JsonValue.JsonObject([.. fields]);
            }

            Document.JsonValue name = InternalParse();
            if (name is not Document.JsonValue.JsonString)
                throw new FormatException($"Expected field name at position: {position}");

            if (IsFinished())
                break;
            
            if (At().Kind != JsonTokenKind.Colon)
                throw new FormatException($"Expected ':' after field name at position: {position}");

            Consume();

            Document.JsonValue value = InternalParse();
            fields.Add((Document.JsonValue.JsonString)name, value);
            gotComma = false;

            if (IsFinished())
                break;

            if (At().Kind == JsonTokenKind.Comma)
            {
                gotComma = true;
                Consume();
            }
        }

        if (gotComma)
            throw new FormatException($"Expected field name after ',' at position: {position}");

        throw new FormatException($"Expected '}}' at end of object, at position: {position}");
    }
}