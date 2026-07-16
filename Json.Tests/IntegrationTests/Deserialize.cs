using System.Security.Cryptography.X509Certificates;
using Json.Serialization;

namespace Json.Tests.IntegrationTests;

[TestClass]
[TestCategory("Deserialize")]
public sealed class DeserializationTests
{
    [TestMethod]
    public void DeserializeSingleValues()
    {
        Assert.AreEqual(100, JsonDeserializer.Deserialize<int>("100"));
        Assert.AreEqual(9999999999999, JsonDeserializer.Deserialize<long>("9999999999999"));
        Assert.AreEqual(-2.5E+100, JsonDeserializer.Deserialize<double>("-2.5E+100"));
        Assert.AreEqual(31.2m, JsonDeserializer.Deserialize<decimal>("31.2"));
        Assert.AreEqual("apples", JsonDeserializer.Deserialize<string>("\"apples\""));
        Assert.IsNull(JsonDeserializer.Deserialize<string>("null"));
    }

    [TestMethod]
    public void DeserializeToNonNullable()
    {
        Assert.Throws<ArgumentException>(() => JsonDeserializer.Deserialize<int>("null"));
        Assert.Throws<ArgumentException>(() => JsonDeserializer.Deserialize<long>("null"));
        Assert.Throws<ArgumentException>(() => JsonDeserializer.Deserialize<double>("null"));
        Assert.Throws<ArgumentException>(() => JsonDeserializer.Deserialize<decimal>("null"));
        Assert.Throws<ArgumentException>(() => JsonDeserializer.Deserialize<bool>("null"));
    }

    [TestMethod]
    public void DeserializeArray()
    {
        var list = JsonDeserializer.Deserialize<List<object>>("[1, 2, true, false, \"apples\", null]");
        Assert.HasCount(6, list);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.IsTrue((bool)list[2]);
        Assert.IsFalse((bool)list[3]);
        Assert.AreEqual("apples", list[4]);
        Assert.IsNull(list[5]);
    }

    private class SomeClass
    {
        public SomeClass()
        {
            PublicNumber = 0;
            privateNumber = 0;
        }

        public int PublicNumber;
        private readonly int privateNumber;

        public int AddBothNumbers() => PublicNumber + privateNumber;
    }

    [TestMethod]
    public void DeserializeObject()
    {
        var obj = JsonDeserializer.Deserialize<SomeClass>("{\"PublicNumber\": 5}");
        Assert.AreEqual(5, obj.PublicNumber);
    }
}