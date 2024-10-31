// <copyright file="Program.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics;
using MD5;

Console.WriteLine("Hello, World!");
int n = 50;
var sequentialTimes = new double[n];
var parallelTimes = new double[n];
var stopwatch = new Stopwatch();
var path = Console.ReadLine();
var checkSum = new CheckSum();
for (var j = 0; j < n; j++)
    {
        stopwatch.Start();
        checkSum.CalculateSequential(path);
        stopwatch.Stop();
        sequentialTimes[j] = stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
        stopwatch.Start();
        await checkSum.CalculateParallel(path);
        stopwatch.Stop();
        parallelTimes[j] = stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
        Console.WriteLine($"At time {j}: Sequential Calculate hash path - {sequentialTimes[j]} ms, Parallel Calculate hash path - {parallelTimes[j]} ms");
    }
