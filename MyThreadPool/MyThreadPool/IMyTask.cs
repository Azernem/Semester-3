// <copyright file="Program.cs" company="NematMusaev">
// Copyright (c) Nemat Musaev. All rights reserved.
// </copyright>

namespace MyThreadPool;
/// <summary>
/// MyTask interface.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IMyTask<out TResult>
{
    /// <summary>
    /// Is finished function or not.
    /// </summary>
    bool IsFinished { get; }

    /// <summary>
    /// Result of task.
    /// </summary>
    TResult Result { get; }

    /// <summary>
    /// transformered task.
    /// </summary>
    /// <typeparam name="TNewResult">type of result of new task.</typeparam>
    /// <param name="func">function.</param>
    /// <returns>TnewResult.</returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
}
