// <copyright file="MethodResult.cs" company="NematMusaev">
// Copyright (c) Nemat Musaev. All rights reserved.
// </copyright>
using MyNUnit;
using System.Text.Json.Serialization;

namespace MyNUnitWeb.Server.Data;

public class MethodResult
{

    public MethodResult()
    {

    }

    public MethodResult(TestReport testReport)
    {
        MethodName = testReport.MethodName;
        Status = testReport.State;
        FailureReason = testReport.Reason;
        Expected = testReport.Expected?.ToString();
        ActualValue = testReport.Was?.ToString();
        TimeElapsed = testReport.TimeElapsed;
    }

    public int MethodResultId { get; set; }
    public string MethodName { get; set; }
    public TestResult Status { get; set; }
    public string? FailureReason { get; set; }

    public string? Expected { get; set; }
    public string? ActualValue { get; set; }
    public long TimeElapsed { get; set; }
    [JsonIgnore]
    public int ClassResultId { get; set; }
}
