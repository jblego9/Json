namespace Json;

/// <summary>
/// Reads in a JSON string and produces a list of <see cref="JsonToken"/>.
/// <para>Ensures the tokens themselves are correct, but not token order correctness.</para>
/// </summary>
public class JsonLexer
{
    public static List<JsonToken> Tokenise(string source)
    {
        var lexer = new JsonLexer(source);
        lexer.InternalTokenise();
        return lexer.tokens;
    }

    private JsonLexer(string source)
    {
        this.source = source;
    }

    private readonly List<JsonToken> tokens = [];
    private readonly string source;
    private int position = 0;

    private void InternalTokenise()
    {
        while (!IsFinished())
        {
            char c = At();
            switch (c)
            {
                case '"': HandleString(); break;
                case '{': Push(JsonTokenKind.OpeningBrace, "{"); Advance(); break;
                case '}': Push(JsonTokenKind.ClosingBrace, "}"); Advance(); break;
                case '[': Push(JsonTokenKind.OpeningBracket, "["); Advance(); break;
                case ']': Push(JsonTokenKind.ClosingBracket, "]"); Advance(); break;
                case ':': Push(JsonTokenKind.Colon, ":"); Advance(); break;
                case ',': Push(JsonTokenKind.Comma, ","); Advance(); break;
                default:
                    if (char.IsWhiteSpace(c)) SkipWhitespace();
                    else if (char.IsLetter(c)) HandleLiteral();
                    else if (c == '-'|| char.IsDigit(c)) HandleNumber();
                    else throw new FormatException($"Unexpected character '{c}' at position: {position}");
                    break;
            }
        }
    }

    private bool IsFinished() => position >= source.Length;
    private char At() => source[position];
    private void Advance(ushort amount = 1) => position += amount;
    private void Push(JsonTokenKind kind, string value = "") => tokens.Add(new JsonToken(kind, value));

    private void SkipWhitespace()
    {
        while (!IsFinished() && char.IsWhiteSpace(At()))
            Advance();
    }

    private void HandleString()
    {
        Advance(); // Skip opening quotation mark.

        string value = "";
        while (!IsFinished())
        {
            char c = At();

            if (c == '"')
            {
                Advance(); // Skip closing quotation mark.
                Push(JsonTokenKind.String, value);
                return;
            }

            value += c;
            Advance();
        }

        throw new FormatException($"Unterminated string \"{value}\" at position: {position}");
    }

    private void HandleLiteral()
    {
        string value = "" + At();
        Advance();

        while (!IsFinished())
        {
            char c = At();

            if (!char.IsLetter(c)) break;

            value += c;
            Advance();
        }

        if (value == "true")
        {
            Push(JsonTokenKind.True, "true");
            return;
        }
        else if (value == "false")
        {
            Push(JsonTokenKind.False, "false");
            return;
        }
        else if (value == "null")
        {
            Push(JsonTokenKind.Null, "null");
            return;
        }

        throw new FormatException($"Invalid literal '{value}' at position: {position}");
    }

    private void HandleNumber()
    {
        string value = "" + At();
        Advance();

        while (!IsFinished())
        {
            char c = At();

            if (!char.IsDigit(c) && c != '.' && c != '-' && c != '+' && char.ToLower(c) != 'e')
                break;
            
            value += c;
            Advance();
        }

        var number = new JsonValue.JsonNumber(value);
        bool valid = false;

        // Ensures the number is valid in atleast one form.
        if (number.TryGetDouble(out _) || number.TryGetDecimal(out _) || number.TryGetInt32(out _) || number.TryGetInt64(out _))
            valid = true;

        // Ensure negative number starts with a digit.
        // If first minus symbol is in the exponent, it must have a digit after it anyway.
        if (value.Contains('-'))
        {
            int minusIndex = value.IndexOf('-');
            if (minusIndex + 1 < value.Length)
            {
                if (!char.IsDigit(value[minusIndex + 1]))
                    valid = false;
            }
        }
        
        if (!valid)
            throw new FormatException($"Invalid number '{value}' at position: {position}");
        
        Push(JsonTokenKind.Number, value);
    }
}