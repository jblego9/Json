# Json

A Json library, built in layers.

## Architecture

### Parsing
Low-level tokenising and parsing. The `JsonLexer` turns raw JSON text into tokens, then the `JsonParser` creates a `JsonValue` tree out of those tokens. Not intended to be used directly, but instead through the `Document` layer.

### Document
Wraps the `Parsing` layer. The `JsonDocument` can both parse and write JSON. Parsing takes in raw JSON text and returns a `JsonValue` tree. Writing takes in a manually created `JsonValue` tree and returns raw JSON text.

### Serialization
Does not exist yet...