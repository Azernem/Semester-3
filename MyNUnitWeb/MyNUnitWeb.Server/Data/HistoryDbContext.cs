using Microsoft.EntityFrameworkCore;

namespace MyNUnitWeb.Server.Data;

public class HistoryDbContext : DbContext
{
    public DbSet<AssemblyResult> Assemblies => Set<AssemblyResult>();
    public DbSet<ClassResult> Classes => Set<ClassResult>();
    public DbSet<MethodResult> Methods => Set<MethodResult>();
    
    public HistoryDbContext(DbContextOptions<HistoryDbContext> options)
        : base(options)
    {

    }
}
