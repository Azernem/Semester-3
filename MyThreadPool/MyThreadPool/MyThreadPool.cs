// <copyright file="MyThreadPool.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.Threading;
using System.Collections.Generic;

namespace MyThreadPool;

/// <summary>
/// pool of threads wich are working with tasks.
/// </summary>
/// <typeparam name="Tres">Result of functions in threads(queue). </typeparam>
public class MyThreadPool<Tres>
{
    private Thread[] pool;

    private CancellationTokenSource cancellationToken = new();

    public Queue<Func<Tres>> queue = new();

    public MyThreadPool(int value)
    {
        pool = new Thread[value];

        for (var i = 0; i < value; i++)
        {
            pool[i] = new Thread(() => {GetTask(); });
        }

        foreach (var thread in pool)
        {
            thread.Start();
        }
    }

    private void GetTask()
    {
        while(! cancellationToken.IsCancellationRequested)
        {
            lock(queue)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queue);
                }

                queue.Dequeue()();
            }
        }

        if (cancellationToken.IsCancellationRequested)
        {
            while(queue.Count > 0)
            {
                lock(queue)
                {
                    queue.Dequeue()();
                }
            }
        }
    }

    public MyTask<Tres> Submit(Func<Tres> func)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            throw new Exception("pool are shut dawn");
        }   

        var myTask = new MyTask<Tres>(func);
        lock(queue)
        {
            queue.Enqueue(myTask.function);
            Monitor.Pulse(queue);
        }

        return myTask;
    }

    public void Shutdawn()
    {
        cancellationToken.Cancel();
        lock(queue)
        {
            Monitor.PulseAll(queue);
        }

        foreach(var thread in pool)
        {
            thread.Join();
        }
    }
}