using Json.Parsing;

namespace Json.Document;

public static class JsonDocument
{
    public static JsonValue Parse(string source) => JsonParser.Parse(JsonLexer.Tokenise(source));
}