// <copyright file="LazyEvaluation.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LazyEvaluation;

using System.Threading;

/// <summary>
/// Class of simple Lazy evulation.
/// </summary>
/// <typeparam name="T">general type.</typeparam>
public class Lazy<T> : ILazy<T>
{
    private T? value;
    private volatile bool isCalculated;
    private Func<T>? supplier;

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="supplier">Function wich returns tupe T.</param>
    /// <exception cref="NullException">Null exception.</exception>
    public Lazy(Func<T> supplier)
    {
        this.isCalculated = false;
        this.supplier = supplier ?? throw new ArgumentNullException("its null element");
    }

    /// <summary>
    /// Method wich gets value.
    /// </summary>
    /// <returns>General type. </returns>
    public T Get()
    {
        if (!this.isCalculated)
        {
            this.value = this.supplier();
            this.isCalculated = true;
            this.supplier = null;
        }

        return this.value ?? throw new InvalidOperationException("Value hasnt been calculated.");
    }
}