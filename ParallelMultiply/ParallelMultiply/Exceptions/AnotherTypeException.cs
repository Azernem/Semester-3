// <copyright file="AnotherTypeException.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParallelMultiplication;

/// <summary>
/// Another Type Exception.
/// </summary>
public class AnotherTypeException : Exception
{
    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="message">mesage. </param>
    public AnotherTypeException(string message): base(message)
    {
    }
}