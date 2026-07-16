using Json.Serialization;

namespace Json.Tests.IntegrationTests;

[TestClass]
[TestCategory("Deserialize")]
public sealed class DeserializeTests
{
    [TestMethod]
    public void DeserializeSingleValues()
    {
        Assert.AreEqual("apples", JsonDeserializer.Deserialize<string>("\"apples\""));
        Assert.AreEqual(100, JsonDeserializer.Deserialize<int>("100"));
        Assert.AreEqual(999999999999, JsonDeserializer.Deserialize<long>("999999999999"));
        Assert.AreEqual(22.5m, JsonDeserializer.Deserialize<decimal>("22.5"));
        Assert.AreEqual(-2.5E+100, JsonDeserializer.Deserialize<double>("-2.5E+100"));
        Assert.IsTrue(JsonDeserializer.Deserialize<bool>("true"));
        Assert.IsFalse(JsonDeserializer.Deserialize<bool>("false"));
        Assert.IsNull(JsonDeserializer.Deserialize<string>("null"));
    }

    [TestMethod]
    public void DeserializeUnsupportedType()
    {
        Assert.Throws<ArgumentException>(() => JsonDeserializer.Deserialize<DateTime>("\"16-07-2026\""));
    }
}