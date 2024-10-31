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

/// <summary>
/// class of tests about MD5.
/// </summary>
public class CheckSumTests
{
    private readonly CheckSum checkSum = new CheckSum();

    /// <summary>
    /// returns correct hash throught sequentail calculate.
    /// </summary>
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
    
    /// <summary>
    /// returns correct hash throught sequentail calculate.
    /// </summary>
    /// <returns></returns>
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
