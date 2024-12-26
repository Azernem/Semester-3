using MyNUnit;
using System.Text.Json.Serialization;

namespace MyNUnitWeb.Server.Data;

public class ClassResult
{
    public ClassResult(ClassReport classReport)
    {
        ClassName = classReport.ClassName;
        Status = classReport.State;

        if (Status == MyNUnit.ClassResult.FAILED)
        {
            FailureReason = classReport.Reason;
        }
        else
        {
            if (classReport.TestReports == null || classReport.TestReports.Count == 0)
            {
                MethodResults = new List<MethodResult>();
                return;
            }

            MethodResults = new List<MethodResult>();
            foreach (var testReport in classReport.TestReports)
            {
                var methodResult = new MethodResult(testReport);
                MethodResults.Add(methodResult);
            }
        }
    }

    public int AssemblyResultId { get; set; }
    public int ClassResultId { get; set; }
    public string ClassName { get; set; }
    public MyNUnit.ClassResult Status { get; set; }
    public string? FailureReason { get; private set; }
    public List<MethodResult>? MethodResults { get; set; }
}
