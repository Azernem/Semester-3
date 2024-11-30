// <copyright file="ExistenceFileLoadException.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParallelMultiplication;

/// <summary>
/// Existence File Load Exception.
/// </summary>
public class ExistenceFileLoadException : Exception
{
    /// <summary>
    /// constructer.
    /// </summary>
    /// <param name="message">mesage exception. </param>
    /// <param name="message">message exception.</param>
    public ExistenceFileLoadException(string message)
    : base(message)
    {
    }
}