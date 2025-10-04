// <copyright file="CompatibilityException.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace FileMatrix;

/// <summary>
/// Сompatibility Exception.
/// </summary>
public class СompatibilityException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="СompatibilityException"/> class.
    /// </summary>
    /// <param name="message">message.</param>
    public СompatibilityException(string message) 
    : base(message)
    {
    }
}