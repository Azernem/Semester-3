// <copyright file="Exceptions.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace ParallelMultiplication;

public class IncorrrectSizeException: Exception
{
    public IncorrrectSizeException(string message): base(message)
    {

    }
}

public class AnotherTypeException: Exception
{
    public AnotherTypeException(string message): base(message)
    {

    }
}

public class СompatibilityException: Exception
{
    public СompatibilityException(string message): base(message)
    {

    }
}

public class ExistenceFileLoadException: Exception
{
    public ExistenceFileLoadException(string message): base(message)
    {

    }
}

public class EmptyFileException: Exception
{
    public EmptyFileException(string message): base(message)
    {

    }
}