namespace Json.Parsing;

public enum JsonTokenKind
{
    String, // Opening and closing quotation marks not included, but implied.
    Number,
    True,
    False,
    Null,
    OpeningBrace, // {
    ClosingBrace, // }
    OpeningBracket, // [
    ClosingBracket, // ]
    Colon,
    Comma,
}