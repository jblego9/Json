# Json
A Json library, built in layers. Includes some tests.

## Architecture

### Parsing Layer
Low-level tokenising and parsing. The `JsonLexer` turns raw JSON text into tokens. The `JsonParser` creates a `JsonValue` tree out of those tokens. Not intended to be used directly, but instead through the `Document` layer.

### Document Layer
Wraps the `Parsing` layer. The `JsonDocument` can both parse and write JSON. Parsing takes in raw JSON text and returns a `JsonValue` tree. Writing takes in a manually created `JsonValue` tree and returns raw JSON text.

### Serialization Layer
Serialization available through the `JsonSerializer`, supports a select few types, see `JsonSerializer` for supported types. Deserialization available through the `JsonDeserializer`, supports a select few types, see `JsonSerializer` for supported types. Intended to simplify converting to and from C# types and JSON.

## Testing

### Json.Tests
Run `dotnet test` to run all tests in `Json.Tests`.

Tests Include:
- Unit Tests
- Integration Tests

### Json.TestProgram
Run `dotnet run` on `Json.TestProgram` to run the console test program.

Uses `JsonDocument` to parse JSON read from file, and to write JSON to file.