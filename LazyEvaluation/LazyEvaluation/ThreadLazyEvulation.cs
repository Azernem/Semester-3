// <copyright file="ThreadLazyEvulation.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace LazyEvaluation;

/// <summary>
/// Class of thread Lazy evulation.
/// </summary>
/// <typeparam name="T">General type.</typeparam>
public class ThreadLazy<T> : ILazy<T>
{
    private T? value;
    private volatile bool isCalculated;
    private object lockObject;
    private Func<T>? supplier;

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="supplier">Function wich returns tupe Tю\.</param>
    /// <exception cref="NullException">Null exception.</exception>
    public ThreadLazy(Func<T> supplier)
    {
        this.isCalculated = false;
        this.supplier = supplier ?? throw new ArgumentNullException("its null element");
        this.lockObject = new object();
    }

    /// <summary>
    /// Method wich gets value.
    /// </summary>
    /// <returns>General tupe T.</returns>
    public T Get()
    {
        if (!this.isCalculated)
        {
            lock (this.lockObject)
            {
                if (!this.isCalculated)
                {
                    this.value = this.supplier();
                    this.isCalculated = true;
                    this.supplier = null;
                }
            }
        }

        return this.value ?? throw new InvalidOperationException("Value has not been calculated.");
    }
}