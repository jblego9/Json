using Json.Serialization;

namespace Json.Tests.IntegrationTests;

[TestClass]
[TestCategory("Serialize")]
public sealed class SerializeTests
{
    [TestMethod]
    public void SerializeSingleValues()
    {
        Assert.AreEqual("100", JsonSerializer.Serialize(100));
        Assert.AreEqual("9999999999", JsonSerializer.Serialize(9999999999));
        Assert.AreEqual("122.5", JsonSerializer.Serialize(122.5));
        Assert.AreEqual("-2.5E+100", JsonSerializer.Serialize(-2.5E+100));
        Assert.AreEqual("\"hello\"", JsonSerializer.Serialize("hello"));
        Assert.AreEqual("true", JsonSerializer.Serialize(true));
        Assert.AreEqual("false", JsonSerializer.Serialize(false));
        Assert.AreEqual("null", JsonSerializer.Serialize(null));
    }

    [TestMethod]
    public void SerializeList()
    {
        List<object?> list = [1, true, false, null, 100, -2.5E+100, "hello"];
        Assert.AreEqual("[1, true, false, null, 100, -2.5E+100, \"hello\"]", JsonSerializer.Serialize(list));
    }

    [TestMethod]
    public void SerializeDictionary()
    {
        var dictionary = new OrderedDictionary<string, object?> {
            {"ammo", 100},
            {"a", true},
            {"b", false},
            {"null", null},
            {"int", 100},
            {"d", -2.5E+100},
            {"text", "hello"}
        };
        Assert.AreEqual(
            "{\"ammo\": 100, \"a\": true, \"b\": false, \"null\": null, \"int\": 100, \"d\": -2.5E+100, \"text\": \"hello\"}",
            JsonSerializer.Serialize(dictionary)
        );
    }

    private class Square(double sideLength)
    {
        public readonly double SideLength = sideLength;

        public double Area() => SideLength * SideLength;
    }

    [TestMethod]
    public void SerializeSquareObject()
    {
        var square = new Square(5);
        Assert.AreEqual("{\"SideLength\": 5}", JsonSerializer.Serialize(square));
    }

    private record Product(string Name, decimal Price);

    [TestMethod]
    public void SerializeProductObjects()
    {
        List<Product> products = [
            new Product("Apple", 2.25m),
            new Product("Orange", 3.00m),
            new Product("Grape", 0.20m)
        ];
        Assert.AreEqual("[{\"Name\": \"Apple\", \"Price\": 2.25}, {\"Name\": \"Orange\", \"Price\": 3.00}, {\"Name\": \"Grape\", \"Price\": 0.20}]", JsonSerializer.Serialize(products));
    }

    private struct SomeStruct(int publicNumber, int privateNumber)
    {
        public int PublicNumber = publicNumber;
        private readonly int privateNumber = privateNumber;

        public readonly int AddBothNumbers() => PublicNumber + privateNumber;
    }

    [TestMethod]
    public void SerializeSomeStruct()
    {
        var value = new SomeStruct(5, 10);
        Assert.AreEqual("{\"PublicNumber\": 5}", JsonSerializer.Serialize(value));
    }
}