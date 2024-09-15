// <copyright file="LazyEvaluation.Tests.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.Diagnostics;
using LazyEvaluation;

namespace LazyEvaluation.Tests;

/// <summary>
/// class of Lazy evulation tests.
/// </summary>
public class Tests
{
    [Test]
    public void MeasureTimeLazyEvilation()
    {
        var times = new double[2];
        var oneTreadOperation = new Lazy<int>(() => 2 * 5 / 2344443 * 45335);
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        oneTreadOperation.Get();
        stopwatch.Stop();
        times[0] = stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
        stopwatch.Start();
        oneTreadOperation.Get();
        stopwatch.Stop();
        times[1] = stopwatch.ElapsedMilliseconds;
        Assert.That(times[1] == times[0]);
    }

    [Test]
    public void CompareFunctionCalls()
    {
        var oneThreadOperation = new Lazy<double>(() => 2 * 5 / 2344443 * 45335);
        Assert.That(oneThreadOperation.flag, Is.EqualTo(false));
        oneThreadOperation.Get();
        Assert.That(oneThreadOperation.flag, Is.EqualTo(true));
    }

    [Test]
    public void TreadCompareFunctionCalls()
    {
        var threadLazyOperation = new ThreadLazy<double>(() => 2 * 5 / 2344443 * 45335);   
        Assert.That(threadLazyOperation.flag, Is.EqualTo(false));
        var threads = new Thread[2];
        var results = new bool[2];
        threadLazyOperation.Get();
        
        for (var i = 0; i < threads.Length; i++)
        {
            var locall = i;
            threads[i] = new Thread(() => {
                results[locall] = threadLazyOperation.flag; 
                threadLazyOperation.Get();
            });
        }
        
        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.That(results.Count(j => j == false) == 0);

    }

    [Test]
    public void GetSameValues()
    {
        var value = 1;
        var lazyOperation = new Lazy<int>(() => {
            value++;

            return value;
        });
        Assert.That(lazyOperation.Get(), Is.EqualTo(2));
        Assert.That(lazyOperation.Get(), Is.EqualTo(2));
        Assert.That(lazyOperation.Get(), Is.EqualTo(2));
    }

    [Test]
    public void ThreadGetSameValues()
    {
        var value = 1;
        var results = new int[4];
        var threadLazyOperation = new ThreadLazy<int>(() => {
            value++;

            return value;
        });
        var threads = new Thread[4];

        for (int i = 0; i < threads.Length; i++)
        {
            var locall = i;
            threads[locall] = new Thread(() => {
                results[locall] = threadLazyOperation.Get();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.That(results.Any(value => value == 2));
    }
}