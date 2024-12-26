using MyNUnit;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNUnitWeb.Server.Data;

public class AssemblyResult
{
    public AssemblyResult(AssemblyReport assemblyReport)
    {
        AssemblyName = assemblyReport.assemblyName;
        foreach (var classReport in assemblyReport.classReports)
        {
            var classResult = new ClassResult(classReport);
            ClassResults.Add(classResult);
        }
        foreach (var classResult in ClassResults)
        {
            if (classResult.MethodResults == null)
            {
                continue;
            }
            foreach (var methodResult in classResult.MethodResults)
            {
                switch (methodResult.Status)
                {
                    case TestResult.PASSED:
                        PassedTest++;
                        break;
                    case TestResult.FAILED:
                        FailedTest++;
                        break;
                    case TestResult.IGNORED:
                        IgnoredTest++;
                        break;
                }
            }
        }
    }

    public int PassedTest { get; private set; }
    public int FailedTest { get; private set; }
    public int IgnoredTest { get; private set; }
    public int TestAssemblyId { get; set; }
    public string Name { get; set; } = ""; 
    public List<TestResult> TestResults { get; set; } = new();
    [NotMapped]
    public int TestsNumber => PassedTest + FailedTest + IgnoredTest;

}
