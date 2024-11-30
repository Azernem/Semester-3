// <copyright file="ParallelMultiply.Tests.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParallelMultiply.Tests;

using ParallelMultiplication;

/// <summary>
/// tests matrixes and operation of matrixes.
/// </summary>
public class Tests
{
    /// <summary>
    /// test about parallel and sequentally multiplication.
    /// </summary>
    [Test]
    public void ParallelAndSequentallyMultiplyTest()
    {
        var checkMatrix = new int[,] { { 5, 12 }, { 1, 8 } };
        string path1 = "../../../matrix1.txt";
        string path2 = "../../../matrix2.txt";
        var fileMatrix1 = new FileMatrix(path1);
        var fileMatrix2 = new FileMatrix(path2);
        Assert.That(MatrixOperation.ParallelMultiply(fileMatrix1.GetMatrix(), fileMatrix2.GetMatrix()), Is.EqualTo(checkMatrix));
        Assert.That(MatrixOperation.SequentiallyMultiply(fileMatrix1.GetMatrix(), fileMatrix2.GetMatrix()), Is.EqualTo(checkMatrix));
    }

    /// <summary>
    /// Test about correct size matrix.
    /// </summary>
    [Test]
    public void IncorrectSizeMatrixTest()
    {
        string path1 = "../../../IncorrectSize.txt";
        Assert.Throws<IncorrrectSizeException>(() => new FileMatrix(path1));
    }

    /// <summary>
    /// Test about right size matrix.
    /// </summary>
    [Test]
    public void SizeMatrixTest()
    {
        string path1 = "../../../Size.txt";
        var fileMatrix = new FileMatrix(path1);
        Assert.That(fileMatrix.GetMatrix().GetLength(0), Is.EqualTo(2));
        Assert.That(fileMatrix.GetMatrix().GetLength(1), Is.EqualTo(3));
    }

    /// <summary>
    /// Test about relevant element type of matrix.
    /// </summary>
    [Test]
    public void CheckMatrixTypeTest()
    {
        string path1 = "../../../CheckMatrixType.txt";
        Assert.Throws<AnotherTypeException>(() => new FileMatrix(path1));
    }

    /// <summary>
    /// Test about compatibility of two matrixes.
    /// </summary>
    [Test]
    public void CompatibilityExceptionTest()
    {
        string path1 = "../../../matrix1.txt";
        string path2 = "../../../Size.txt";
        var fileMatrix1 = new FileMatrix(path1);
        var fileMatrix2 = new FileMatrix(path2);
        Assert.Throws<СompatibilityException>(() => MatrixOperation.ParallelMultiply(fileMatrix1.GetMatrix(), fileMatrix2.GetMatrix()));
    }

    /// <summary>
    /// Test wich generates matrix.
    /// </summary>
    [Test]
    public void GenerateMatrixTest()
    {
        var (sizeRows, sizeColumns) = (3, 3);
        var matrix = FileMatrix.GenerateMatrix(sizeRows, sizeColumns);
        Assert.That(matrix.GetLength(0), Is.EqualTo(sizeRows));
    }

    /// <summary>
    /// Checkes compatibility of matrixes.
    /// </summary>
    [Test]
    public void EqualsTest()
    {
        var matrix1 = new int[,] { { 1, 2, 3 }, { 3, 3, 3 } };
        var matrix2 = new int[,] { { 1, 2 }, { 3, 3 } };
        Assert.Throws<СompatibilityException>(() => MatrixOperation.Compare(matrix1, matrix2));
    }

    /// <summary>
    /// Gets file with particular matrix.
    /// </summary>
    [Test]
    public void GetFileWithResult()
    {
        var resultPath = "../../../resultmatrix.txt";
        var checkMatrix = new int[,] { { 5, 12 }, { 1, 8 } };
        string path1 = "../../../matrix1.txt";
        string path2 = "../../../matrix2.txt";
        var fileMatrix1 = new FileMatrix(path1);
        var fileMatrix2 = new FileMatrix(path2);
        FileMatrix.WriteToFile(MatrixOperation.ParallelMultiply(fileMatrix1.GetMatrix(), fileMatrix2.GetMatrix()), resultPath);
        var fileResultMatrix = new FileMatrix(resultPath);
        var resultMatrix = fileResultMatrix.GetMatrix();
        Assert.That(resultMatrix, Is.EqualTo(checkMatrix));
    }

    /// <summary>
    /// Gets same responces at different methodes of multiplication.
    /// </summary>
    [Test]
    public void GetSameResponces()
    {
        string path1 = "../../../matrix1.txt";
        string path2 = "../../../matrix2.txt";
        var fileMatrix1 = new FileMatrix(path1);
        var fileMatrix2 = new FileMatrix(path2);
        var matrix1 = fileMatrix1.GetMatrix();
        var matrix2 = fileMatrix2.GetMatrix();
        Assert.That(MatrixOperation.ParallelMultiply(matrix1, matrix2), Is.EqualTo(MatrixOperation.SequentiallyMultiply(matrix1, matrix2)));
    }
}