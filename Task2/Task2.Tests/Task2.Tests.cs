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
        /// <summary>
        /// field in class.
        /// </summary>
        public int Field;

        /// <summary>
        /// private field in class.
        /// </summary>
        private string privateField;
    }

    /// <summary>
    /// get differences between two classes.
    /// </summary>
    [Test]
    public void DiffClassesTest()
    {
        var classA = typeof(ClassA);
        var classB = typeof(ClassB);
        string expectedDifference = "String privateField\n" +
                                    "Int32 Field2\n" +
                                    "Method2";
        string result = Reflector.DiffClasses(classA, classB);
        Assert.IsTrue(result.Contains(expectedDifference));
    }

    /// <summary>
    /// classA.
    /// </summary>
    public class ClassA
    {
        /// <summary>
        /// Field.
        /// </summary>
        public int Field;

        /// <summary>
        /// privateField.
        /// </summary>
        private string privateField;

        /// <summary>
        /// Method.
        /// </summary>
        public void Method() { }
    }

    /// <summary>
    /// classB.
    /// </summary>
    public class ClassB
    {
        /// <summary>
        /// Field.
        /// </summary>
        public int Field;

        /// <summary>
        /// Field2.
        /// </summary>
        public int Field2;

        /// <summary>
        /// Method.
        /// </summary>
        public void Method() { }

        /// <summary>
        /// Method2.
        /// </summary>
        public void Method2() { }
    }
}
