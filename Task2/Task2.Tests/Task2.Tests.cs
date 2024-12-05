// <copyright file="Task2.Tests.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Task2.Tests;

/// <summary>
/// class of tests.
/// </summary>
public class Tests
{
    /// <summary>
    /// checks members of my class at file.
    /// </summary>
    [Test]
    public void PrintStructure_CreatesFileWithClassDefinition()
    {
        var reflector = new Reflector();
        var testClassType = typeof(TestClass);
        var fileName = "TestClass.cs";
        reflector.PrintStructure(testClassType);

        var fileContent = File.ReadAllText(fileName);
        Assert.IsTrue(fileContent.Contains("public class TestClass"));
        Assert.IsTrue(fileContent.Contains("public int Field;"));
        Assert.IsTrue(fileContent.Contains("private string field;"));
    }

    /// <summary>
    /// my class.
    /// </summary>
    public class TestClass
    {
        public int field;
        private string privateField;
    }
}
