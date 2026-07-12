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
            switch (At())
            {
                case '"': HandleString(); break;
                default: {
                    if (char.IsWhiteSpace(At()))
                        SkipWhitespace();
                    else
                        throw new FormatException($"Unexpected character '{At()}' at position: {position}");

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

    // TODO: Handle escape sequences.
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
}