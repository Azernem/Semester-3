// <copyright file="IncorrectSizeException.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace FileMatrix;

/// <summary>
/// Incorrrect Size Exception.
/// </summary>
public class IncorrrectSizeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncorrrectSizeException"/> class.
    /// </summary>
    /// <param name="message">message.</param>
    public IncorrrectSizeException(string message)
    : base(message)
    {
    }
}