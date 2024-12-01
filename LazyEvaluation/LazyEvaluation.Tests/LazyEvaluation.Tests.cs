// <copyright file="LazyEvaluation.Tests.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LazyEvaluation.Tests;

using System.Diagnostics;
using LazyEvaluation;

/// <summary>
/// class of Lazy evulation tests.
/// </summary>
public class Tests
{

    /// <summary>
    /// same working of threads without parallel.
    /// </summary>
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

    /// <summary>
    /// same working of threads with parallel.
    /// </summary>
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