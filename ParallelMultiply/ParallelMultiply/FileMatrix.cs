// <copyright file="FileMatrix.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace ParallelMultiplication;

using System.Text;
using System.Threading;

/// <summary>
/// Creates and generate matrixes, gets file with matrix.
/// </summary>
public class FileMatrix
{
    /// <summary>
    /// Write matrix to file.
    /// </summary>
    /// <param name="matrix"> Matrix wich will be written to file. </param>
    /// <param name="path"> Path with matrix. </param>
    public static void WriteToFile(int[,] matrix, string path)
    {
        var stringList = new List<string> { };
        var line = new StringBuilder();

        for (var i = 0; i < matrix.GetLength(0); ++i)
        {
            for (var j = 0; j < matrix.GetLength(1); ++j)
            {
                line.Append($"{matrix[i, j]} ");
            }

            stringList.Add(line.ToString()[..^1]);
            line = new StringBuilder();
        }

        File.WriteAllLines(path, stringList);
    }

    /// <summary>
    /// Generate matrix.
    /// </summary>
    /// <param name="sizeRows">Size of matrix at rows. </param>
    /// <param name="sizeColumns">Size of matrix at columns.</param>
    /// <returns> Generated matrix.</returns>
    public static int[,] GenerateMatrix(int sizeRows, int sizeColumns)
    {
        var random = new Random();
        var matrix = new int[sizeRows, sizeColumns];

        for (var i = 0; i < sizeRows; i++)
        {
            for (var j = 0; j < sizeColumns; j++)
            {
                matrix[i, j] = random.Next(1, 50);
            }
        }

        return matrix;
    }

    /// <summary>
    /// matrix in file.
    /// </summary>
    public int[,] matrix;
    private string path;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="path">path where there is matrix. </param>
    public FileMatrix(string path)
    {
        this.path = path;
        this.matrix = new int[0, 0];
        this.CreateMatrix();
    }

    /// <summary>
    /// Creates matrix with particular file and gets exception.
    /// </summary>
    /// <exception cref="ExistenceFileLoadException"> Existence file load exception. </exception>
    /// <exception cref="EmptyFileException"> Empty file exception. </exception>
    /// <exception cref="IncorrrectSizeException"> Incorrrect Size Exception. </exception>
    /// <exception cref="AnotherTypeException"> Another Type Exception. </exception>
    private void CreateMatrix()
    {
        if (!File.Exists(this.path))
        {
            throw new ExistenceFileLoadException("file doesnt exist");
        }

        var strings = File.ReadAllLines(this.path);
        this.matrix = new int[strings.Length, strings[0].Split(new char[] { ' ', ',' }).Length];

        if (strings.Length == 0)
        {
            throw new EmptyFileException();
        }

        for (int i = 0; i < strings.Length; i++)
        {
            if (!(strings[i].Split(new char[] { ' ', ',' }).Length == this.matrix.GetLength(1)) || strings[0] == String.Empty)
            {
                throw new IncorrrectSizeException("Incorrect size");
            }

            for (int j = 0; j < this.matrix.GetLength(1); j++)
            {
                if (!int.TryParse(strings[i].Split(new char[] { ' ', ',' })[j], out this.matrix[i, j]))
                {
                    throw new AnotherTypeException("Incorrect size");
                }
            }
        }
    }
}
