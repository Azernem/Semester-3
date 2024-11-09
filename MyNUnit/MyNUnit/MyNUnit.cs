// <copyright file="MyNUnit.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace MyNUnit.Tests;

using System.Diagnostics;
using System.Reflection;

/// <summary>
/// my NUnit.
/// </summary>
public static class Tests
{
    /// <summary>
    /// Test attribute.
    /// </summary>
    public class TestAttribute : Attribute
    {
        /// <summary>
        /// Argument about exception.
        /// </summary>
        public static Type Expected;

        /// <summary>
        /// Is ignored method.
        /// </summary>
        public static string Ignore;
    }

    /// <summary>
    /// befor class attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class BeforeClass : Attribute
    {
    }

    /// <summary>
    /// after class attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AfterClass : Attribute
    {
    }

    /// <summary>
    /// Before attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Before : Attribute
    {
    }

    /// <summary>
    /// after attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class After : Attribute
    {
    }

    /// <summary>
    /// main method of running.
    /// </summary>
    /// <param name="path">path with types. </param>
    /// <exception cref="ArgumentException">argument exception. </exception>
    public static void Run(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new ArgumentException("path must be without directories");
        }

        foreach (var dll in Directory.GetFiles(path, "*dll."))
        {
            var assembly = Assembly.Load(dll);

            foreach (var type in assembly.GetTypes())
            {
                foreach (var methodinfo in type.GetMethods())
                {
                    foreach (var attribute in methodinfo.GetCustomAttributes())
                    {
                        if (attribute.GetType() == typeof(TestAttribute))
                        {
                            BeforeRun(type);
                            TestRun(type);
                            AfterRun(type);
                            break;
                        }
                    }
                }
            }
        }
    }

    private static void BeforeRun(Type type)
    {
        var beforeClassMethodes = type.GetMethods().Where(m => m.GetCustomAttributes(typeof(BeforeClass), false).Any());

        foreach (var method in beforeClassMethodes)
        {
            method.Invoke(null, null);
        }

        var beforeMethodes = type.GetMethods().Where(m => m.GetCustomAttributes(typeof(Before), false).Any());

        foreach (var method in beforeMethodes)
        {
            method.Invoke(Activator.CreateInstance(type), null);
        }
    }

    private static void TestRun(Type type)
    {
        var tasks = new List<Task>();
        var testmethodes = new List<MethodInfo>();

        foreach (var methodinfo in type.GetMethods())
        {
            foreach (var attribute in methodinfo.GetCustomAttributes())
            {
                if (attribute.GetType() == typeof(TestAttribute))
                {
                    testmethodes.Add(methodinfo);
                }
            }
        }

        foreach (var method in testmethodes)
        {
                tasks.Add(Task.Run(() => {
                    if (!string.IsNullOrEmpty(TestAttribute.Ignore))
                    {
                        var stopwatch = new Stopwatch();
                        try
                        {
                            stopwatch.Start();
                            method.Invoke(Activator.CreateInstance(type), null);
                            stopwatch.Stop();
                            Console.WriteLine($"Test {method.Name} was completed for {stopwatch.ElapsedMilliseconds} - ms, cause is {TestAttribute.Ignore}");
                            stopwatch.Reset();
                        }
                        catch (Exception ex)
                        {
                            stopwatch.Stop();

                            if (TestAttribute.Expected == ex.GetType())
                            {
                                Console.WriteLine($"Test {method.Name} was failed for {stopwatch.ElapsedMilliseconds} - ms ");
                            }

                            stopwatch.Reset();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Test {method.Name} was ignored.");
                    }
                }));
        }

        Task.WhenAll(tasks);
    }

    private static void AfterRun(Type type)
    {
        var afterClassMethodes = type.GetMethods().Where(m => m.GetCustomAttributes(typeof(AfterClass), false).Any());

        foreach (var method in afterClassMethodes)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            method.Invoke(null, null);
            stopwatch.Stop();
            Console.WriteLine($"AfterClass test {method.Name} was completed for {stopwatch.ElapsedMilliseconds} - ms");
            stopwatch.Reset();
        }

        var afterMethodes = type.GetMethods().Where(m => m.GetCustomAttributes(typeof(After), false).Any());

        foreach (var method in afterMethodes)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            method.Invoke(Activator.CreateInstance(type), null);
            stopwatch.Stop();
            Console.WriteLine($"After test was completed for {stopwatch.ElapsedMilliseconds} - ms");
            stopwatch.Reset();
        }
    }
}
