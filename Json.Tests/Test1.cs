namespace Json.Tests;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestMethod1()
    {
        bool result = Class1.GetTrue();
        Assert.IsTrue(result);
    }
}
