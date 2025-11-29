// <copyright file="LazyInterface.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace LazyEvaluation;

/// <summary>
/// Interface of Lazy evulation classes.
/// </summary>
/// <typeparam name="T">General Type.</typeparam>
public interface ILazy<out T>
{
    /// <summary>
    /// Gets value supplier.
    /// </summary>
    /// <returns>value.</returns>
    T Get();
}