// <copyright file="AnotherTypeException.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace FileMatrix;
/// <summary>
/// Another Type Exception. invalid integer exception.
/// </summary>
public class InvalidIntegerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidIntegerException"/> class.
    /// </summary>
    /// <param name="message">message.</param>
    public InvalidIntegerException(string message)
    : base(message)
    {
    }
}