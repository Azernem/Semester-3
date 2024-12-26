// <copyright file="MyNUnit.cs" company="NematMusaev">
// Copyright (c) Nemat Musaev. All rights reserved.
// </copyright>

namespace MyNUnit
{
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// MyNUnit.
    /// </summary>
    public class MyNUnit
    {
        private readonly string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyNUnit"/> class.
        /// </summary>
        /// <param name="path">Path to the assemblies to test.</param>
        public MyNUnit(string path)
        {
            path = path;
        }

        /// <summary>
        /// Runs tests in the assemblies.
        /// </summary>
        /// <returns>A list of assembly reports.</returns>
        /// <exception cref="ArgumentException">Thrown if the path is invalid.</exception>
        public List<AssemblyReport> RunTests()
        {
            if (!Directory.Exists(path))
            {
                throw new ArgumentException("Path must exist and contain assemblies.");
            }

            var assemblyReports = new List<AssemblyReport>();

            foreach (var dll in Directory.GetFiles(path, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(dll);
                var assemblyReport = new AssemblyReport
                {
                    AssemblyName = Path.GetFileName(dll)
                };

                foreach (var type in assembly.GetTypes())
                {
                    var classReport = RunTestsForType(type);
                    if (classReport != null)
                    {
                        assemblyReport.ClassReports.Add(classReport);
                    }
                }

                assemblyReports.Add(assemblyReport);
            }

            return assemblyReports;
        }

        /// <summary>
        /// Runs tests for a type.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>A class report with test results.</returns>
        private ClassReport? RunTestsForType(Type type)
        {
            if (!ExecuteStaticMethods<BeforeClass>(type))
            {
                return new ClassReport
                {
                    ClassName = type.Name,
                    State = ClassResult.FAILED,
                    Reason = "Error in BeforeClass methods."
                };
            }

            var testReports = new List<TestReport>();
            foreach (var method in type.GetMethods().Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any()))
            {
                var testReport = ExecuteTest(method, type);
                testReports.Add(testReport);
            }

            ExecuteStaticMethods<AfterClass>(type);

            return new ClassReport
            {
                ClassName = type.Name,
                State = testReports.Any(r => r.State == TestResult.FAILED) ? ClassResult.FAILED : ClassResult.PASSED,
                TestReports = testReports
            };
        }

        /// <summary>
        /// Executes a specific test method.
        /// </summary>
        /// <param name="method">The method to execute.</param>
        /// <param name="type">The type containing the method.</param>
        /// <returns>A report of the test result.</returns>
        private TestReport ExecuteTest(MethodInfo method, Type type)
        {
            var testAttribute = method.GetCustomAttribute<TestAttribute>();
            var instance = Activator.CreateInstance(type);
            var stopwatch = new Stopwatch();

            var report = new TestReport
            {
                MethodName = method.Name,
                State = TestResult.FAILED
            };

            if (testAttribute?.Ignore != null)
            {
                report.State = TestResult.IGNORED;
                report.Reason = testAttribute.Ignore;
                return report;
            }

            try
            {
                ExecuteMethods<Before>(type, instance);
                stopwatch.Start();

                try
                {
                    method.Invoke(instance, null);
                    stopwatch.Stop();
                    report.State = TestResult.PASSED;
                }
                catch (TargetInvocationException ex)
                {
                    stopwatch.Stop();
                    var actualException = ex.InnerException;
                    if (testAttribute.Expected != null && actualException?.GetType() == testAttribute.Expected)
                    {
                        report.State = TestResult.PASSED;
                        report.Reason = "Expected exception was thrown.";
                    }
                    else
                    {
                        report.Reason = actualException?.Message;
                    }
                }
                finally
                {
                    ExecuteMethods<After>(type, instance);
                }
            }
            catch (Exception ex)
            {
                report.Reason = ex.Message;
            }

            report.TimeElapsed = stopwatch.ElapsedMilliseconds; 
            return report;
        }

        /// <summary>
        /// Executes all methods with a specific attribute.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="type">The type containing the methods.</param>
        /// <param name="instance">The instance of the type.</param>
        private static void ExecuteMethods<T>(Type type, object? instance)
        where T : Attribute
        {
            foreach (var method in type.GetMethods().Where(m => m.GetCustomAttributes(typeof(T), false).Any()))
            {
                try
                {
                    method.Invoke(instance, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in method {method.Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Executes all static methods with a specific attribute.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="type">The type containing the methods.</param>
        /// <returns>True if all methods executed successfully, otherwise false.</returns>
        private static bool ExecuteStaticMethods<T>(Type type)
        where T : Attribute
        {
            foreach (var method in type.GetMethods().Where(m => m.GetCustomAttributes(typeof(T), false).Any()))
            {
                try
                {
                    method.Invoke(null, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in static method {method.Name}: {ex.Message}");
                    return false;
                }
            }

            return true;
        }
    }

    public class TestReport
    {
        public string MethodName { get; set; }
        public TestResult State { get; set; }
        public string? Reason { get; set; }
        public object? Expected { get; set; }
        public object? Was { get; set; }
        public long TimeElapsed { get; set; }
    }

    public class ClassReport
    {
        public string ClassName { get; set; }
        public ClassResult State { get; set; }
        public string? Reason { get; set; }
        public List<TestReport>? TestReports { get; set; }
    }

    public class AssemblyReport
    {
        public string AssemblyName { get; set; }
        public List<ClassReport> ClassReports { get; set; } = new();
    }

    public enum TestResult
    {
        PASSED,
        FAILED,
        IGNORED
    }

    public enum ClassResult
    {
        PASSED,
        FAILED
    }
}
