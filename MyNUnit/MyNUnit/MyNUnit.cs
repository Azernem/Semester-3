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
        private readonly string? path;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyNUnit"/> class.
        /// </summary>
        /// <param name="path">Path to the assemblies to test.</param>
        public MyNUnit(string path)
        {
            path = path ?? throw new ArgumentNullException("path cant be empty");
        }

        /// <summary>
        /// Runs tests in the assemblies.
        /// </summary>
        /// <returns>A list of assembly reports.</returns>
        /// <exception cref="ArgumentException">Thrown if the path is invalid.</exception>
        public List<AssemblyReport> RunTests()
        {
            if (!Directory.Exists(this.path))
            {
                throw new ArgumentException("Path must exist and contain assemblies.");
            }

            var assemblyReports = new List<AssemblyReport>();

            foreach (var dll in Directory.GetFiles(this.path, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(dll);
                var assemblyReport = new AssemblyReport
                {
                    AssemblyName = Path.GetFileName(dll),
                };

                var tasks = new List<Task>();
                foreach (var type in assembly.GetTypes())
                {
                    tasks.Add(Task.Run(() =>
                    {
                    var classReport = this.RunTestsForType(type);
                    if (classReport != null)
                    {
                        assemblyReport.ClassReports.Add(classReport);
                    }
                    }));
                }

                Task.WhenAll(tasks).Wait();

                assemblyReports.Add(assemblyReport);
            }

            return assemblyReports;
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
                    Reason = "Error in BeforeClass methods.",
                };
            }

            var testReports = new List<TestReport>();
            foreach (var method in type.GetMethods().Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any()))
            {
                var testReport = this.ExecuteTest(method, type);
                testReports.Add(testReport);
            }

            if (!ExecuteStaticMethods<AfterClass>(type))
            {
                return new ClassReport
                {
                    ClassName = type.Name,
                    State = ClassResult.FAILED,
                    Reason = "Error in AfterClass methods.",
                };
            }

            return new ClassReport
            {
                ClassName = type.Name,
                State = testReports.Any(r => r.State == TestResult.FAILED) ? ClassResult.FAILED : ClassResult.PASSED,
                TestReports = testReports,
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
                State = TestResult.FAILED,
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
                    if (testAttribute.Expected != null)
                    {
                        report.State = TestResult.FAILED;
                        report.Reason = "Expected exception wasnt thrown.";
                    }
                }
                catch (TargetInvocationException ex)
                {
                    stopwatch.Stop();
                    var actualException = ex.InnerException;
                    if (testAttribute != null && testAttribute.Expected != null && actualException?.GetType() == testAttribute.Expected)
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
                report.State = TestResult.FAILED;
            }

            report.TimeElapsed = stopwatch.ElapsedMilliseconds;
            return report;
        }
    }

    /// <summary>
    /// TestReport class.
    /// </summary>
    public class TestReport
    {
        /// <summary>
        /// Gets or sets Name if method.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets Status of Method.
        /// </summary>
        public TestResult State { get; set; }

        /// <summary>
        /// Gets or sets Reason of failing.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Gets or sets expected exception.
        /// </summary>
        public object? Expected { get; set; }

        /// <summary>
        /// Gets or sets Was.
        /// </summary>
        public object? Was { get; set; }

        /// <summary>
        /// Gets or sets Time exuting.
        /// </summary>
        public long TimeElapsed { get; set; }
    }

    /// <summary>
    /// class results.
    /// </summary>
    public class ClassReport
    {
        /// <summary>
        /// Gets or sets Name if method.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets Reason of failing.
        /// </summary>
        public ClassResult State { get; set; }

        /// <summary>
        /// Gets or setsReason of failing.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Gets or setsMethod results.
        /// </summary>
        public List<TestReport>? TestReports { get; set; }
    }

    /// <summary>
    /// assembly result.
    /// </summary>
    public class AssemblyReport
    {
        /// <summary>
        /// nGets or sets ame of assembly.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets class results.
        /// </summary>
        public List<ClassReport> ClassReports { get; set; } = new ();
    }

    /// <summary>
    /// emun of test reults.
    /// </summary>
    public enum TestResult
    {
        PASSED,
        FAILED,
        IGNORED
    }

    /// <summary>
    /// emun of class reults.
    /// </summary>
    public enum ClassResult
    {
        PASSED,
        FAILED,
    }
}
