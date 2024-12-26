// <copyright file="MyThreadPool.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace MyThreadPool;

/// <summary>
/// Class of task.
/// </summary>
/// <typeparam name="Tres">return type of task. </typeparam>
public class MyTask<Tres> : IMyTask<Tres>
{
    public bool IsCompleted {get; set; }

    public Func<Tres> function;

    private List<Action<Tres>> continuations = new List<Action<Tres>>();

    private Exception? exception;

    public Tres Result => GetResult();

    public MyTask(Func<Tres> func)
    {
        IsCompleted = false;
        this.function = func;
    }

    private Tres GetResult()
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

        if (result is null)
        {
            throw new ArgumentException("the function cant be null");
        }

        else if (exception is not null)
        {
            throw new AggregateException(exception);
        }

        else
        {
            IsCompleted = true;
            RunContinuations(result);
            return result;
        }
    }

    public IMyTask<TNewRes> ContinueWith<TNewRes>(Func<Tres, TNewRes> func)
    {
        var continuationTask = new MyTask<TNewRes>(() => func(this.Result));

        continuations.Add(result =>
        {
            continuationTask.function();
        });

        return continuationTask;
    }

    private void RunContinuations(Tres result)
    {
        foreach (var continuation in continuations)
        {
            continuation(result);
        }
    }
}
