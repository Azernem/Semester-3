using MyNUnit;
using System.Text.Json.Serialization;

namespace MyNUnitWeb.Server.Data;

public class MethodResult
{
    public MethodResult(TestReport testReport)
    {
        MethodName = testReport.methodName;
        Status = testReport.state;
        Reason = testReport.reason;
        TimeElapsed = testReport.timeElapsed;
        Expected = testReport.expected?.ToString();
        Was = testReport.was?.ToString();
    }

    public int MethodResultId { get; set; }
    public string MethodName { get; set; }
    public long TimeElapsed { get; set; }
    public int ClassResultId { get; set; }
    public TestResult Status { get; set; }
    public string? Reason { get; set; }

    public string? Expected { get; set; }
    public string? Was { get; set; }
    [JsonIgnore]
}
