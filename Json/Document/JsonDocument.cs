namespace Json.Document;

public static class JsonDocument
{
    public static JsonValue Parse(string source) => Parsing.JsonParser.Parse(Parsing.JsonLexer.Tokenise(source));
}