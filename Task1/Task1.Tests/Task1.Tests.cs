// <copyright file="LazyEvaluation.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Task1.Tests;
using MD5;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}

public class CheckSumTests
{
    private readonly CheckSum checkSum = new CheckSum();

    [Test]
    public void CalculateSequentallyFileHashReturnsCorrectHash()
    {
    string testFilePath = "../../../testfile.txt";

    var hash = checkSum.CalculateSequential(testFilePath);

    using var md5 = MD5.Create();
    {
        byte[] checker = md5.ComputeHash(File.ReadAllBytes(testFilePath));
        Assert.That(checker, Is.EqualTo(hash));
    }
    }

    public async Task CalculateParallelFileHashReturnsCorrectHash()
    {
        string testFilePath = "../../../testfile.txt";
        var hash = await checkSum.CalculateParallel(testFilePath);

        using var md5 = MD5.Create();
        {
            byte[] checker = md5.ComputeHash(File.ReadAllBytes(testFilePath));
            Assert.That(checker, Is.EqualTo(hash));
        }
    }
}
