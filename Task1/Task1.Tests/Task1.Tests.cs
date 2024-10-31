// <copyright file="LazyEvaluation.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Task1.Tests;
using MD5;
using System.Security.Cryptography;

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
    private readonly CheckSum checkSum;

    public CheckSumTests()
    {
    checkSum = new CheckSum();
    }

    [Test]
    public void CalculateFileHash_ReturnsCorrectHash()
    {
    // Arrange
    string testFilePath = "testfile.txt";
    File.WriteAllText(testFilePath, "Hello, World!");

    // Act
    byte[] hash = checkSum.CalculateSequential(testFilePath);

    // Assert
    using (var md5 = MD5.Create())
    {
        byte[] expectedHash = md5.ComputeHash(File.ReadAllBytes(testFilePath));
        Assert.That(expectedHash, Is.EqualTo(hash));
    }
    }
}