// <copyright file="IncorrectSizeException.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace ParallelMultiplication;

/// <summary>
/// Incorrrect Size Exception.
/// </summary>
public class IncorrrectSizeException : Exception
{
    /// <summary>
    /// constructer.
    /// </summary>
    /// <param name="message">message.</param>
    public IncorrrectSizeException(string message): base(message)
    {
    }
}