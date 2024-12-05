// <copyright file="Task2.Tests.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Task2.Tests;

public class Tests
{
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

    public class TestClass
    {
        public int Field;

        private string privateField;
    }

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

    public class ClassA
    {
        public int Field;

        private string privateField;

        public void Method() { }
    }
    public class ClassB
    {
        public int Field;

        public int Field2;

        public void Method() { }
        public void Method2() { }
    }
}
