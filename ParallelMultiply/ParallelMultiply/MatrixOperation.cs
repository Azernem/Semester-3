// <copyright file="ParallelMultiply.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.Numerics;
using System;
namespace ParallelMultiplication;

/// <summary>
/// Class about multiplication of two matrixes.
/// </summary>
public class MatrixOperation
{
    public int[,] matrix1;
    public int[,] matrix2;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="matrix1">First matrix. </param>
    /// <param name="matrix2">Second matrix.</param>
    public MatrixOperation(int[,] matrix1, int[,] matrix2)
    {
        this.matrix1 = matrix1;
        this.matrix2 = matrix2;
    }

    /// <summary>
    /// Combine two matrixes at size.
    /// </summary>
    /// <exception cref="СompatibilityException">Сompatibility matrix exceptio</exception>
    public void Combine()
    {
        if (! (matrix1.GetLength(1) == matrix2.GetLength(0)))
        {
            throw new СompatibilityException("matrixes arent equal");
        }
    }
    
    /// <summary>
    /// Sequentally multiply matrixes.
    /// </summary>
    /// <returns>Result matrix</returns>
    public int[,] SequentiallyMultiply()
        {
            Combine();
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
    public int[,] ParallelMultiply()
        {
            Combine();
            var result = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
            var threads = new Thread[Environment.ProcessorCount];
            var blockSize = matrix1.GetLength(0) / threads.Length + 1;

            for (var i = 0; i < threads.Length; ++i)
            {
                var locall = i;
                threads[locall] = new Thread(() =>
                {
                    for (var n = locall*blockSize; n < (locall + 1) * blockSize; n++)
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
