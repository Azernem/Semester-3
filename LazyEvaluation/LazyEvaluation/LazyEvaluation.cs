// <copyright file="LazyEvaluation.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.Threading;

namespace LazyEvaluation;

/// <summary>
/// Class of simple Lazy evulation.
/// </summary>
/// <typeparam name="T">general type</typeparam>
public class Lazy<T>: ILazy<T>
{
    private T value;
    public bool flag;
    private Func<T> supplier;

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="supplier">Function wich returns tupe T.</param>
    /// <exception cref="NullException">Null exception.</exception>
    public Lazy(Func<T> supplier)
    {
        this.flag = false;
        this.supplier = supplier ?? throw new NullException("its null element");
    }

    /// <summary>
    /// Method wich gets value.
    /// </summary>
    /// <returns>General type/</returns>
    public T Get()
    {
        if (! flag)
        {
            value = supplier();
            flag = true;
        }

        return value;
    }
}

/// <summary>
/// Class of thread Lazy evulation.
/// </summary>
/// <typeparam name="T">General type.</typeparam>
public class ThreadLazy<T>: ILazy<T>
{
    private T value;
    public bool flag;
    private object data;
    private Func<T> supplier;

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="supplier">Function wich returns tupe T</param>
    /// <exception cref="NullException">Null exception.</exception>
    public ThreadLazy(Func<T> supplier)
    {
        this.flag = false;
        this.supplier = supplier ?? throw new NullException("its null element");
        this.data = new object();
    }
    
    /// <summary>
    /// Method wich gets value.
    /// </summary>
    /// <returns>General tupe T.</returns>
    public T Get()
    {
        if (! flag)
        {
            lock (data)

            if (! flag)
            {
                flag = true;
                value = supplier();
            }
        }

        return value;
    }
}