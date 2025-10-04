// <copyright file="ExistenceFileLoadException.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileMatrix;

/// <summary>
/// Existence File Load Exception.
/// </summary>
public class ExistenceFileLoadException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExistenceFileLoadException"/> class.
    /// </summary>
    /// <param name="message">message.</param>
    public ExistenceFileLoadException(string message)
    : base(message)
    {
    }
}