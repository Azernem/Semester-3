// <copyright file="MyThreadPool.Tests.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace MyThreadPool.Tests;

using MyThreadPool;
using System.Collections;

/// <summary>
/// Class of pool tests.
/// </summary>
public class Tests
{
    /// <summary>
    /// Right working of submit method.
    /// </summary>
    [Test]
    public void TestResultOfTasks()
    {
        var pool = new ThreadPool(4);
        var check = new int[4] { 1, 2, 3, 4 };
        var result = new int[4];
        result[0] = pool.Submit(() => 1).Result;
        result[1] = pool.Submit(() => 2).Result;
        result[2] = pool.Submit(() => 3).Result;
        result[3] = pool.Submit(() => 4).Result;
        Assert.That(result, Is.EqualTo(check));
    }

    /// <summary>
    /// Submit transeforming to class mytask.
    /// </summary>
    [Test]
    public void GetSameTasks()
    {
        var pool = new ThreadPool(4);
        var checkTask = pool.Submit(() => "eeree");
        var myTask = pool.Submit(() => "eeree");
        while (!myTask.IsFinished)
        {
            Thread.Sleep(10);
        }

        Assert.That(checkTask.Result, Is.EqualTo(myTask.Result));
    }

    /// <summary>
    /// Right working of ContinueWithMethod.
    /// </summary>
    [Test]
    public void ContinueWithINThird()
    {
        var pool = new ThreadPool(10);
        var myTask1 = pool.Submit(() => 5);
        var myTask2 = myTask1.ContinueWith(x => x * 0);
        var myTask3 = myTask2.ContinueWith(x => x.ToString());
        var myTask4 = myTask3.ContinueWith(x => x + "");
        Assert.That(myTask3.Result, Is.EqualTo("0"));
    }

    /// <summary>
    /// Invalid operation in submit after cancelling.
    /// </summary>
    [Test]
    public void SubmitAfterShutDown()
    {
        var pool = new ThreadPool(10);
        var checkTask = new ThreadPool.MyTask<string>(() => "eeree", pool);
        pool.ShutDown();
        Assert.Throws<OperationCanceledException>(() => pool.Submit(() => "eeree"));
    }



    /// <summary>
    /// Invalid operation in getting other task after cancelling.
    /// </summary>
    [Test]
    public void ContinueWithAfterShutDown()
    {
        var pool = new ThreadPool(10);
        var checkTask = pool.Submit(() => 2 + 3);
        pool.ShutDown();
        Assert.Throws<OperationCanceledException>(() => checkTask.ContinueWith<string>(x => x.ToString()));
    }
} 