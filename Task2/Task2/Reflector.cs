// <copyright file="Task2.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Task2;

using System.Reflection;
using System.Text;

/// <summary>
/// reflecrot of classes.
/// </summary>
public class Reflector
{
    /// <summary>
    /// gets differences between two classes at fields and methodes.
    /// </summary>
    /// <param name="a">type of a class.</param>
    /// <param name="b">type of class b.</param>
    /// <returns>string of differences.</returns>
    public static string DiffClasses(Type a, Type b)
    {
        StringBuilder stringBuilder = new StringBuilder();

        var aFields = a.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        var bFields = b.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        var aMethods = a.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        var bMethods = b.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        stringBuilder.AppendLine($"differences between {a.Name} and {b.Name}:");

        foreach (var field in aFields)
        {
            if (!bFields.Any(t => t.Name == field.Name && t.FieldType == field.FieldType))
            {
                stringBuilder.AppendLine($"{field.FieldType.Name} {field.Name}");
            }
        }

        foreach (var field in bFields)
        {
            if (!aFields.Any(f => f.Name == field.Name && f.FieldType == field.FieldType))
            {
                stringBuilder.AppendLine($"{field.FieldType.Name} {field.Name}");
            }
        }

        foreach (var method in aMethods)
        {
            if (!bMethods.Any(m => m.Name == method.Name && m.ReturnType == method.ReturnType && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(method.GetParameters().Select(p => p.ParameterType))))
            {
                stringBuilder.AppendLine($"{method.Name}");
            }
        }

        foreach (var method in bMethods)
        {
            if (!aMethods.Any(m => m.Name == method.Name && m.ReturnType == method.ReturnType && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(method.GetParameters().Select(p => p.ParameterType))))
            {
                stringBuilder.AppendLine($"{method.Name}");
            }
        }

        Console.WriteLine(stringBuilder.ToString());

        return stringBuilder.ToString();
    }

    /// <summary>
    /// prints structure.
    /// </summary>
    /// <param name="someClass">my class.</param>
    public void PrintStructure(Type someClass)
    {
        var fileName = $"{someClass.Name}.cs";
        using (var writer = new StreamWriter(fileName))
        {
            writer.WriteLine($"public class {someClass.Name}");
            writer.WriteLine("{");
            this.PrintClass(writer, someClass);
            writer.WriteLine();
        }
    }

    private static void PrintFields(StreamWriter writer, Type someClass)
    {
        var fields = someClass.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        foreach (var fieldInfo in fields)
        {
            PrintModifiers(writer, fieldInfo);
            writer.Write($"{GetInfoType(fieldInfo.FieldType)} ");
            writer.Write($"{fieldInfo.Name};");
            writer.WriteLine();
        }

        writer.WriteLine();
    }

    private static void PrintMethods(StreamWriter writer, Type someClass)
    {
        var methods = someClass.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        foreach (var methodInfo in methods)
        {
            PrintModifiers(writer, methodInfo);
            writer.Write($"{GetInfoType(methodInfo.ReturnType)} ");
            var parameters = methodInfo.GetParameters().Select(p => $"{GetInfoType(p.ParameterType)} {p.Name}");
            var stringParameters = string.Join(", ", parameters);
            writer.Write($"{methodInfo.Name}({stringParameters});");
            writer.WriteLine();
        }

        writer.WriteLine();
    }

    private static void PrintModifiers(StreamWriter writer, MemberInfo memberInfo)
    {
        var stringBuilder = new StringBuilder();
        if (memberInfo is FieldInfo)
        {
            var type = memberInfo as FieldInfo;

            if (type.IsPublic)
            {
                stringBuilder.Append("public ");
            }

            if (type.IsPrivate)
            {
                stringBuilder.Append("private ");
            }

            if (type.IsStatic)
            {
                stringBuilder.Append("static ");
            }

            if (type.IsFamily)
            {
                stringBuilder.Append("protected ");
            }
        }
        else
        {
            var type = memberInfo as MethodInfo;

            if (type.IsPublic)
            {
                stringBuilder.Append("public ");
            }

            if (type.IsPrivate)
            {
                stringBuilder.Append("private ");
            }

            if (type.IsStatic)
            {
                stringBuilder.Append("static ");
            }

            if (type.IsFamily)
            {
                stringBuilder.Append("protected ");
            }
        }

        writer.Write(stringBuilder.ToString());
    }

    private static string GetInfoType(Type memberType)
    {
        var stringBuilder = new StringBuilder();
        var typeName = memberType.Name;
        stringBuilder.Append(typeName);

        if (memberType.IsGenericType)
        {
            stringBuilder.Append(typeName.Substring(0, typeName.IndexOf("`")));
            stringBuilder.Append("<");
            var generices = memberType.GetGenericArguments().Select(arg => GetInfoType(arg));
            stringBuilder.Append(string.Join(", ", generices));
            stringBuilder.Append("> ");
        }

        return stringBuilder.ToString();
    }

    private void PrintClass(StreamWriter writer, Type someClass)
    {
        PrintFields(writer, someClass);
        PrintMethods(writer, someClass);
        this.PrintInterfaces(writer, someClass);

        var inheritedClasses = someClass.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var inheritedClass in inheritedClasses)
        {
            this.PrintClass(writer, inheritedClass);
        }

        writer.WriteLine("}");
    }

    private void PrintInterfaces(StreamWriter writer, Type someClass)
    {
        var interfaces = someClass.GetInterfaces();

        foreach (var interfaceType in interfaces)
        {
            writer.WriteLine($"public interface {interfaceType.Name}");
            writer.WriteLine("{");
            PrintMethods(writer, interfaceType);
            writer.WriteLine("}");
        }

        writer.WriteLine();
    }

}