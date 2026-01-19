using MyNUnit;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNUnitWeb.Server.Data;

public class AssemblyResult
{

    /// <summary>
    /// Default constructor.
    /// </summary>
    public AssemblyResult() { }

    /// <summary>
    /// Initializes a new instance of the AssemblyResult class with specified data.
    /// </summary>
    /// <param name="report">The test report containing assembly data.</param>
    public AssemblyResult(AssemblyReport report)
    {
        AssemblyName = report.AssemblyName;
        ClassResults = report.ClassReports.Select(classReport => new ClassResult(classReport)).ToList();
    }

     public int AssemblyResultId { get; set; }
    public string AssemblyName { get; set; } = string.Empty;
    public long ExecutionTime { get; set; }
    public bool IsSuccessful { get; set; }
    public List<ClassResult> ClassResults { get; set; } = new();
}
