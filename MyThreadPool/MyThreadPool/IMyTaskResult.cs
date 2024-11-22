// <copyright file="IMyTaskResult.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.Collections;

namespace MyThreadPool;

/// <summary>
/// Interface of task with Result.
/// </summary>
/// <typeparam name="Tres">Result of task. </typeparam>
public interface IMyTask<Tres>
{
    public bool IsCompleted {get; set;}

    public Tres? Result {get; }

    public IMyTask<TNewRes> ContinueWith<TNewRes>(Func<Tres, TNewRes> func);
}