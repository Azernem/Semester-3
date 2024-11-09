// <copyright file="Exceptions.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace ParallelMultiplication;

/// <summary>
/// Incorrrect Size Exception.
/// </summary>
public class IncorrrectSizeException : Exception
{
    public IncorrrectSizeException(string message): base(message)
    {
    }
}

/// <summary>
/// Another Type Exception.
/// </summary>
public class AnotherTypeException : Exception
{
    public AnotherTypeException(string message): base(message)
    {
    }
}

/// <summary>
/// Сompatibility Exception.
/// </summary>
public class СompatibilityException: Exception
{
    public СompatibilityException(string message): base(message)
    {
    }
}

/// <summary>
/// Existence File Load Exception.
/// </summary>
public class ExistenceFileLoadException : Exception
{
    public ExistenceFileLoadException(string message): base(message)
    {
    }
}

/// <summary>
/// Empty File Exception.
/// </summary>
public class EmptyFileException : Exception
{
    public EmptyFileException(string message): base(message)
    {
    }
}