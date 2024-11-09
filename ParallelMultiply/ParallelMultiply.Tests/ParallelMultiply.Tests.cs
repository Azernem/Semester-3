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
    /// test about parallel multiplication.
    /// </summary>
    [Test]
    public void ParallelMultiplyTest()
    {
        var checkMatrix = new int[,] {{5, 12}, {1, 8}};
        string path1 = "../../../matrix1.txt";
        string path2 = "../../../matrix2.txt";
        var fileMatrix1 = new FileMatrix(path1);
        fileMatrix1.CreateMatrix();
        var fileMatrix2 = new FileMatrix(path2);
        fileMatrix2.CreateMatrix();
        var matrixOperation = new MatrixOperation(fileMatrix1.matrix, fileMatrix2.matrix);
        Assert.That(matrixOperation.ParallelMultiply(), Is.EqualTo(checkMatrix));
    }

    /// <summary>
    /// Test about parallel multiplication.
    /// </summary>
    [Test]
    public void SequentallyMultiplyTest()
    {
        var checkMatrix = new int[,] { { 5, 12 }, { 1, 8 } };
        string path1 = "../../../matrix1.txt";
        string path2 = "../../../matrix2.txt";
        var fileMatrix1 = new FileMatrix(path1);
        fileMatrix1.CreateMatrix();
        var fileMatrix2 = new FileMatrix(path2);
        fileMatrix2.CreateMatrix();
        var matrixOperation = new MatrixOperation(fileMatrix1.matrix, fileMatrix2.matrix);
        Assert.That(matrixOperation.SequentiallyMultiply(), Is.EqualTo(checkMatrix));
    }
    
    /// <summary>
    /// Test about correct size matrix.
    /// </summary>
    [Test]
    public void IncorrectSizeMatrixTest()
    {
        string path1 = "../../../IncorrectSize.txt";
        var fileMatrix1 = new FileMatrix(path1);
        Assert.Throws<IncorrrectSizeException>(() => fileMatrix1.CreateMatrix());
    }
    
    /// <summary>
    /// Test about right size matrix.
    /// </summary>
    [Test]
    public void SizeMatrixTest()
    {
        string path1 = "../../../Size.txt";
        var fileMatrix = new FileMatrix(path1);
        fileMatrix.CreateMatrix();
        Assert.That(fileMatrix.matrix.GetLength(0), Is.EqualTo(2));
        Assert.That(fileMatrix.matrix.GetLength(1), Is.EqualTo(3));
    }
    
    /// <summary>
    /// Test about relevant element type of matrix.
    /// </summary>
    [Test]
    public void CheckMatrixTypeTest()
    {
        string path1 = "../../../CheckMatrixType.txt";
        var fileMatrix = new FileMatrix(path1);
        Assert.Throws<AnotherTypeException>(() => fileMatrix.CreateMatrix());
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
        fileMatrix1.CreateMatrix();
        var fileMatrix2 = new FileMatrix(path2);
        fileMatrix2.CreateMatrix();
        var matrixOperation = new MatrixOperation(fileMatrix1.matrix, fileMatrix2.matrix);
        Assert.Throws<СompatibilityException>(() => matrixOperation.ParallelMultiply());
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
        var matrix1 = new int[,] {{1, 2 , 3}, {3, 3, 3}};
        var matrix2 = new int[,] {{1, 2}, {3, 3}};
        var matrixOperation = new MatrixOperation(matrix1, matrix2);
        Assert.Throws<СompatibilityException>(() => matrixOperation.Combine());
    }
    
    /// <summary>
    /// Gets file with particular matrix.
    /// </summary>
    [Test]
    public void GetFileWithResult()
    {
        var resultPath = "../../../resultmatrix.txt"; 
        var checkMatrix = new int[,] {{5, 12}, {1, 8}};
        string path1 = "../../../matrix1.txt";
        string path2 = "../../../matrix2.txt";
        var fileMatrix1 = new FileMatrix(path1);
        fileMatrix1.CreateMatrix();
        var fileMatrix2 = new FileMatrix(path2);
        fileMatrix2.CreateMatrix();
        var matrixOperation = new MatrixOperation(fileMatrix1.matrix, fileMatrix2.matrix);
        FileMatrix.WriteToFile(matrixOperation.ParallelMultiply(), resultPath);
        var fileResultmatrix = new FileMatrix(resultPath);
        fileResultmatrix.CreateMatrix();
        var resultmatrix = fileResultmatrix.matrix;
        Assert.That(resultmatrix, Is.EqualTo(checkMatrix));
    }

    /// <summary>
    /// Gets same responces at different methodes of multiplication.
    /// </summary>
    [Test]
    public void GetSameResponces()
    {
        string path1 = "../../../matrix1.txt";
        string path2 = "../../../matrix2.txt";
        var filematrix1 = new FileMatrix(path1);
        var filematrix2 = new FileMatrix(path2);
        filematrix1.CreateMatrix();
        filematrix2.CreateMatrix();
        var matrix1 = filematrix1.matrix;
        var matrix2 = filematrix2.matrix;
        var matrixOperation = new MatrixOperation(matrix1, matrix2);
        Assert.That(matrixOperation.ParallelMultiply(), Is.EqualTo(matrixOperation.SequentiallyMultiply()));
    }
}