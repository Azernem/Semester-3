// <copyright file="CompatibilityException.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParallelMultiplication;

/// <summary>
/// Сompatibility Exception.
/// </summary>
public class СompatibilityException : Exception
{
    /// <summary>
    /// constructer.
    /// </summary>
    /// <param name="message">message.</param>
    public СompatibilityException(string message): base(message)
    {
    }
}