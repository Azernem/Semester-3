// <copyright file="CompatibilityException.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace FileMatrix;

/// <summary>
/// Сompatibility Exception.
/// </summary>
public class CompatibilityException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompatibilityException"/> class.
    /// </summary>
    /// <param name="message">message.</param>
    public CompatibilityException(string message)
    : base(message)
    {
    }
}