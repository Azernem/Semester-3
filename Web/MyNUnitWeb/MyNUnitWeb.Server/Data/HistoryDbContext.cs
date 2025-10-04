// <copyright file="HistoryDBContext.cs" company="NematMusaev">
// Copyright (c) Nemat Musaev. All rights reserved.
// </copyright>
using Microsoft.EntityFrameworkCore;

namespace MyNUnitWeb.Server.Data;

// DbContext for the history of test results
public class HistoryDbContext : DbContext
{
    // Constructor accepting DbContextOptions for HistoryDbContext
    public HistoryDbContext(DbContextOptions<HistoryDbContext> options)
        : base(options)
    {
    }

    // DbSet for storing AssemblyResult entities
    public DbSet<AssemblyResult> Assemblies => Set<AssemblyResult>();

    // DbSet for storing ClassResult entities
    public DbSet<ClassResult> Classes => Set<ClassResult>();

    // DbSet for storing MethodResult entities
    public DbSet<MethodResult> Methods => Set<MethodResult>();
}
