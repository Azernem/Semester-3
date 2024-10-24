// <copyright file="MyThreadPool.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using MyThreadPool;
using System.Collections;

namespace MyThreadPool.Tests;
/// <summary>
/// Class of pool tests.
/// </summary>
public class Tests
{
    [Test]
    public void TestResultOfTasks()
    {
        var pool = new MyThreadPool<int>(4);
        var check = new int[4] {1, 2, 3, 4};
        var result = new int[4];
        result[0] = pool.Submit(() => 1).Result;
        result[1] = pool.Submit(() => 2).Result;
        result[2] = pool.Submit(() => 3).Result;
        result[3] = pool.Submit(() => 4).Result;
        Assert.That(result, Is.EqualTo(check));
    }

    [Test]
    public void GetSameTasks()
    {
        var checktask = new MyTask<string>(() => "eeree");
        var pool = new MyThreadPool<string>(4);
        var myTask = pool.Submit(() => "eeree");
        Assert.That(checktask, Is.EqualTo(myTask));
    }
}