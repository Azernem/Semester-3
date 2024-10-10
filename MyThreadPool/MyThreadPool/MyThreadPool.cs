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

    private CancellationTokenSource cancellationToken = new CancellationTokenSource();

    public Queue<Func<Tres>> queue = new Queue<Func<Tres>>();

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
        
        foreach (var thread in pool)
        {
            thread.Join();
        }
    }

    private void GetTask()
    {
        while(! cancellationToken.IsCancellationRequested)
        {
            lock(queue);

            while (queue.Count == 0)
            {
                Monitor.Wait(queue);
            }

            queue.Dequeue()();
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
        if (! cancellationToken.IsCancellationRequested)
        {
            throw new Exception("pool are shut dawn");
        }   

        var myTask = new MyTask<Tres>(func);
        lock(queue)
        queue.Enqueue(myTask.function);

        return myTask;
    }

    public void Shutdawn()
    {
        cancellationToken.Cancel();
        Monitor.PulseAll(queue);

        foreach(var thread in pool)
        {
            thread.Join();
        }
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="Tres">return type of task. </typeparam>
public class MyTask<Tres>: IMyTask<Tres>
{
    public bool IsCompleted {get; set; }

    public Func<Tres> function;

    public Exception? exception;

    public Tres? Result => GetResult();

    public MyTask(Func<Tres> func)
    {
        IsCompleted = false;
        this.function = func;
    }
    
    private Tres? GetResult()
    {
        Tres? result = default;

        try
        {
            result = function();
        }

        catch(Exception ex)
        {
            exception = ex;
        }

        if (! (exception is null))
        {
            IsCompleted = true;
            return result;
        }

        else
        {
            throw new AggregateException(exception);
        }
    }

    public IMyTask<TNewRes> ContinueWith<TNewRes>(Func<Tres, TNewRes> func)
    {
        return new MyTask<TNewRes>(() => func(this.Result));    
    }
}
