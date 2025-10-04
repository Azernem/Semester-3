// <copyright file="Program.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics;
using FileMatrix;
using ParallelMultiplication;

var n = 100;
var sizes = new List<int>();
var parallelTime = new List<double>();
var sequentallyTime = new List<double>();
var stopwatch = new Stopwatch();

for (var i = 20; i < 130; i += 10)
{
    sizes.Add(i);
    var currentSize = i;
    var sequentallyTimes = new double[n];
    var parallelTimes = new double[n];
    double seqStandartDaviation;
    double parallelStandartDaviation;
    var matrix = FileMatrix.FileMatrix.GenerateMatrix(currentSize, currentSize);

    for (var j = 0; j < n; j++)
    {
        stopwatch.Start();
        MatrixOperation.SequentiallyMultiply(matrix, matrix);
        stopwatch.Stop();
        sequentallyTimes[j] = stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
        stopwatch.Start();
        MatrixOperation.ParallelMultiply(matrix, matrix);
        stopwatch.Stop();
        parallelTimes[j] = stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
    }

    var avarageValueSeq = sequentallyTimes.Sum() / n;
    sequentallyTime.Add(avarageValueSeq);
    var avarageValueParallel = parallelTimes.Sum() / n;
    parallelTime.Add(avarageValueParallel);
    var (sequentallyDaviations, parallelDaviations) = (new double[n], new double[n]);

    for (var k = 0; k < n; k++)
    {
        sequentallyDaviations[k] = Math.Pow(sequentallyTimes[k] - avarageValueSeq, 2);
        parallelDaviations[k] = Math.Pow(parallelTimes[k] - avarageValueSeq, 2);
    }

    seqStandartDaviation = sequentallyDaviations.Sum() / n;
    parallelStandartDaviation = parallelDaviations.Sum() / n;
    Console.WriteLine($"At size {i} of matrix: avarage value in multyplication in sequentally - {avarageValueSeq} ms, in parallel -  {avarageValueParallel} ms;  Standard deviation of multiplication sequentally - {seqStandartDaviation} ms, parallel - {parallelStandartDaviation}");
}
