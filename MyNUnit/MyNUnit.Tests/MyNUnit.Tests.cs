using System;
using System.IO;
using NUnit.Framework;
using MyNUnit;

namespace MyNUnit.Tests;

public class SampleTests
{
    public static bool BeforeClassFlag = false;
    public static bool AfterClassFlag = false;

    [BeforeClass]
    public static void BeforeClassTests()
    {
        BeforeClassFlag = true;
    }

    [AfterClass]
    public static void AfterClassTests()
    {
        AfterClassFlag = true;
    }

    [Before]
    public void BeforeTest() { }

    [After]
    public void AfterTest() { }

    [Test]
    public void ShouldSetBeforeClassFlag()
    {
        Assert.IsTrue(BeforeClassFlag);
    }

    [Test]
    public void ShouldNotAfterClassFlagBeforeTest()
    {
        Assert.IsFalse(AfterClassFlag);
    }

    [Test]
    public void ShouldExecuteTestsCorrectly()
    {
        var testDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAssemblies");
        Directory.CreateDirectory(testDirectory);

        var myNUnit = new MyNUnit(testDirectory);
        var reports = myNUnit.RunTests();

        Assert.IsNotNull(reports);
        Assert.IsNotEmpty(reports);
    }
}
