using Microsoft.VisualBasic;

namespace Json;

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
                default: {
                    if (char.IsWhiteSpace(c)) SkipWhitespace();
                    else if (char.IsLetter(c)) HandleLiteral();
                    else throw new FormatException($"Unexpected character '{c}' at position: {position}");

                    break;
                }
            }
        }
    }

    private bool IsFinished() => position >= source.Length;
    private char At() => source[position];

    private bool TryPeek(out char value)
    {
        if (position + 1 >= source.Length) {
            value = ' ';
            return false;
        }
        
        value = source[position + 1];
        return true;
    }

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
        } else if (value == "false")
        {
            Push(JsonTokenKind.False, "false");
            return;
        } else if (value == "null")
        {
            Push(JsonTokenKind.Null, "null");
            return;
        }

        throw new FormatException($"Invalid literal \"{value}\" at position: {position}");
    }
}