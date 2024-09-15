// <copyright file="Exception.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace LazyEvaluation;

/// <summary>
/// Class with null exception.
/// </summary>
public class NullException: Exception
{
    public NullException(string message): base(message)
    {
        
    }
}