// <copyright file="MatrixOperation.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace ParallelMultiplication;

using System.Numerics;
using System;
using FileMatrix;
/// <summary>
/// Class about multiplication of two matrixes.
/// </summary>
public static class MatrixOperation
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="matrix1">First matrix of multiplication, multiplier. </param>
    /// <param name="matrix2">Second matrix of multiplication, multiplier.</param>

    /// <summary>
    /// Compare two matrixes at size.
    /// </summary>
    /// <exception cref="CompatibilityException">Сompatibility matrix exceptio</exception>
    public static void Compare(int[,] matrix1, int[,] matrix2)
    {
        if (!(matrix1.GetLength(1) == matrix2.GetLength(0)))
        {
            throw new CompatibilityException("matrixes arent equal");
        }
    }

    /// <summary>
    /// Sequentally multiply matrixes.
    /// </summary>
    /// <returns>Result matrix. </returns>
    public static int[,] SequentiallyMultiply(int[,] matrix1, int[,] matrix2)
    {
        Compare(matrix1, matrix2);
        var result = new int[matrix1.GetLength(0), matrix2.GetLength(1)];

        for (var i = 0; i < matrix1.GetLength(0); ++i)
        {
            for (var j = 0; j < matrix2.GetLength(1); ++j)
            {
                for (var k = 0; k < matrix1.GetLength(1); ++k)
                {
                    result[i, j] += matrix1[i, k] * matrix2[k, j];
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Parallel multiply matrixes.
    /// </summary>
    /// <returns>Result matrix.</returns>
    public static int[,] ParallelMultiply(int[,] matrix1, int[,] matrix2)
    {
        Compare(matrix1, matrix2);
        var result = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
        var threads = new Thread[Math.Min(Environment.ProcessorCount, matrix1.GetLength(0))];
        var blockSize = matrix1.GetLength(0) / threads.Length + 1;

        for (var i = 0; i < threads.Length; ++i)
        {
            var locall = i;
            threads[locall] = new Thread(() =>
            {
                for (var n = locall * blockSize; n < (locall + 1) * blockSize; n++)
                {
                    if (n >= result.GetLength(0))
                    {
                        break;
                    }

                    for (int j = 0; j < result.GetLength(1); ++j)
                    {
                        for (var k = 0; k < matrix1.GetLength(1); ++k)
                        {
                            result[n, j] += matrix1[n, k] * matrix2[k, j];
                        }
                    }
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return result;
    }
}
