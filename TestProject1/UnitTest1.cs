namespace TestProject1;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        //set up mock serve/handler

    }

    [Test]
    public void TestEncoding_ValuesEqual()
    {
        // tbd
        var testValue = "xyz";
        var expectedValue = "xyz";

        Assert.That(expectedValue, Is.EqualTo(testValue));
    }

    [Test]
    public void TestEncoding_TestValueNotEqual()
    {
        // tbd
        var testValue = "xyz";
        var expectedValue = "wby";

        Assert.That(testValue, Is.Not.EqualTo(expectedValue));
    }

    [Test]
    public void TestDecoding_ValuesEqual()
    {
        // tbd
        var testValue = 12;
        var expectedValue = 12;

        Assert.That(expectedValue, Is.EqualTo(testValue));
    }

    [Test]
    public void TestDecoding_TestValueNotEqual()
    {
        // tbd
        var testValue = 12;
        var expectedValue = 21;

        Assert.That(testValue, Is.Not.EqualTo(expectedValue));
    }

    [Test]
    public void Test_ResponseUrlRequestUrlMatch()
    {
        // tbd
        //test for invalid response url
        var requestUrl = "test";
        var responseUrl = "test";
        Assert.That(requestUrl, Is.EqualTo(responseUrl));
    }

    [Test]
    public void Test_ResponseUrlTestUrlDoNotMatch()
    {
        // tbd
        var requestUrl = "test";
        var responseUrl = "test1";

        Assert.That(requestUrl, Is.Not.EqualTo(responseUrl));
    }
}
