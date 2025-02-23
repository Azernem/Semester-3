using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Server.Data;

namespace MyNUnitWeb.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase 
{
    private readonly HistoryDbContext databaseContext; 
    private readonly DirectoryInfo uploadDirectory;

    public TestController(IWebHostEnvironment environment, HistoryDbContext databaseContext)
    {
        this.databaseContext = databaseContext; 
        uploadDirectory = new DirectoryInfo("uploaded_files"); 
    }

    /// <summary>
    /// Executes tests and returns the results.
    /// </summary>
    [HttpGet]
    [Route("GetTestResults")]
    public async Task<IActionResult> RetrieveTestResults() 
    {
        var testRunner = new MyNUnit.MyNUnit(uploadDirectory.Name);
        var testReports = testRunner.RunTests();
        var assemblyResults = testReports.Select(report => new AssemblyResult(report)).ToArray();

        Parallel.ForEach(assemblyResults, assemblyResult => {
        {
            if (assemblyResult.ClassResults.Count > 0)
            {
                databaseContext.Assemblies.Add(assemblyResult);
            }
            foreach (var classResult in assemblyResult.ClassResults)
            {
                databaseContext.Classes.Add(classResult);
                if (classResult.MethodTestResults == null)
                    continue;
                foreach (var methodResult in classResult.MethodTestResults)
                {
                    databaseContext.Methods.Add(methodResult);
                }
            }
        }
        });

        testRunner = null;
        foreach (var file in uploadDirectory.GetFiles())
        {
            file.Delete();
        }
        await databaseContext.SaveChangesAsync();
        return Redirect("GetHistory");
    }


    /// <summary>
    /// Returns the history of test executions.
    /// </summary>
    [HttpGet]
    [Route("GetHistory")]
    public IActionResult GetHistory() 
    {
        try
        {
            var assemblyResults = databaseContext.Assemblies.ToList();
            foreach (var assembly in assemblyResults)
            {
                var associatedClasses = databaseContext.Classes
                    .Where(cls => cls.AssemblyResultId == assembly.AssemblyResultId) 
                    .ToList();

                foreach (var cls in associatedClasses)
                {
                    var methodResults = databaseContext.Methods
                        .Where(mtd => mtd.ClassResultId == cls.ClassResultId) 
                        .ToList();
                    cls.MethodTestResults = methodResults;
                }

                assembly.ClassResults = associatedClasses;
            }
            assemblyResults.Reverse();

            return Ok(assemblyResults);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    /// <summary>
    /// Uploads test assemblies to the server.
    /// </summary>
    [HttpPost]
    [Route("Upload")]
    public IActionResult UploadTestFiles(IEnumerable<IFormFile> uploadedFiles) 
    {
        try
        {
            foreach (var file in uploadedFiles)
            {
                if (file.Length > 0)
                {
                    string targetPath = Path.Combine(uploadDirectory.Name, file.FileName); 
                    using var fileStream = new FileStream(targetPath, FileMode.Create);
                    file.CopyTo(fileStream);
                }
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}