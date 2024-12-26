using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Server.Data;

namespace MyNUnitWeb.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TestExecutionController : ControllerBase 
{
    private readonly HistoryDbContext contextDatabase; 
    private readonly DirectoryInfo _uploadDirectory;

    public TestExecutionController(IWebHostEnvironment environment, HistoryDbContext databaseContext)
    {
        contextDatabase = databaseContext;
        _uploadDirectory = new DirectoryInfo("wwwroot/tests");
    }

    [HttpGet]
    [Route("RetrieveHistory")]
    public IActionResult RetrieveHistory()
    {
        try
        {
            var assemblies = contextDatabase.Assemblies.ToList();
            foreach (var assembly in assemblies)
            {
                var relatedClasses = contextDatabase.Classes.Where(classResult => classResult.AssemblyResultId == assembly.AssemblyResultId)
                    .ToList();
                foreach (var relatedClass in relatedClasses)
                {
                    relatedClass.MethodResults = contextDatabase.Methods .Where(method => method.ClassResultId == relatedClass.ClassResultId)
                        .ToList();
                }
                assembly.ClassResults = relatedClasses;
            }
            assemblies.Sort((x, y) => y.AssemblyResultId.CompareTo(x.AssemblyResultId)); 
            return Ok(assemblies);
        }
        catch (Exception exception)
        {
            return StatusCode(500, exception.Message);
        }
    }

     [HttpGet]
    [Route("ExecuteTests")]
    public async Task<IActionResult> ExecuteTests()
    {
        var testRunner = new MyNUnit.MyNUnit(_uploadDirectory.FullName);
        var testReports = testRunner.RunTests();
        var assemblyResults = testReports
            .Select(report => new AssemblyResult(report))
            .ToArray();

        foreach (var assemblyResult in assemblyResults)
        {
            if (assemblyResult.ClassResults.Count > 0)
            {
                contextDatabase.Assemblies.Add(assemblyResult);
            }
            foreach (var classResult in assemblyResult.ClassResults)
            {
                contextDatabase.Classes.Add(classResult);
                if (classResult.MethodResults != null)
                {
                    contextDatabase.Methods.AddRange(classResult.MethodResults);
                }
            }
        }

        foreach (var testFile in _uploadDirectory.GetFiles())
        {
            testFile.Delete();
        }

        await contextDatabase.SaveChangesAsync();
        return Ok(new { message = "Tests executed and results saved" });
    }

    [HttpPost]
    [Route("UploadFiles")]
    public IActionResult UploadFiles(IEnumerable<IFormFile> testAssemblies)
    {
        try
        {
            foreach (var testFile in testAssemblies)
            {
                if (testFile.Length > 0)
                {
                    string targetPath = Path.Combine(_uploadDirectory.FullName, testFile.FileName);
                    using var fileStream = new FileStream(targetPath, FileMode.Create);
                    testFile.CopyTo(fileStream);
                }
            }
            return Ok(new { message = "Files uploaded successfully" });
        }
        catch (Exception exception)
        {
            return StatusCode(500, exception.Message);
        }
    }

}
