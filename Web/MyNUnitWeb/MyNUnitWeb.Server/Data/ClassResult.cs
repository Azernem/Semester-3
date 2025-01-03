// <copyright file="ClassResult.cs" company="NematMusaev">
// Copyright (c) Nemat Musaev. All rights reserved.
// </copyright>
using MyNUnit;
using System.Text.Json.Serialization;

namespace MyNUnitWeb.Server.Data;


public class ClassResult
{
    public ClassResult() { }

    /// <summary>
    /// Creates a new instance of the ClassResult class.
    /// </summary>
    /// <param name="classReport">Contains data about the class test results.</param>
    public ClassResult(ClassReport classReport)
    {
        ClassName = classReport.ClassName;
        TestStatus = classReport.State; 
        if (TestStatus == MyNUnit.ClassResult.FAILED)
        {
            FailureReason = classReport.Reason; 
        }
        else
        {
            if (classReport.TestReports == null)
            {
                return;
            }

            MethodTestResults = new(); 
    
            foreach (var report in classReport.TestReports)
            {
                var methodResult = new MethodResult(report);
                MethodTestResults.Add(methodResult);
            }
        }
    }

    /// <summary>
    /// Identifier for the test class result.
    /// </summary>
    public int ClassResultId { get; set; }

    /// <summary>
    /// Name of the test class.
    /// </summary>
    public string ClassName { get; set; }

    MyNUnit.ClassResult TestStatus { get; set; }

    /// <summary>
    /// Reason for test failure, if applicable.
    /// </summary>
    public string? FailureReason { get; private set; }

    /// <summary>
    /// List of results for methods in the test class.
    /// </summary>
    public List<MethodResult>? MethodTestResults { get; set; }

    [JsonIgnore]
    public int AssemblyResultId { get; set; }
}
