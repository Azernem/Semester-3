// <copyright file="LazyEvaluation.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace MD5;

using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Text;
/// <summary>
/// class with checksum.
/// </summary>
public class CheckSum
{
    private MD5 checkSum = MD5.Create();

/// <summary>
/// Sequentally Calculate.
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
    public byte[] CalculateSequential(string path)
    {
        if (File.Exists(path))
        {
            return CalculateFileHash(path);
        }
        else
        {
            return CalculateDirectoryHash(path);
        }
    }

    public async Task<byte[]> CalculateParallel(string path)
    {
        if (File.Exists(path))
        {
            return await CalculateFileHashAsync(path);
        }
        else
        {
            return await CalculateDirectoryHashAsync(path);
        }
    }

    private byte[] CalculateFileHash(string path)
    {
        var bytes = File.ReadAllBytes(path);
        return checkSum.ComputeHash(bytes);
    }

    private byte[] CalculateDirectoryHash(string path)
    {
        long result = 0;
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);
        
        Array.Sort(directories);
        
        for (int i = 0; i < directories.Length; i++)
        {
            result += BitConverter.ToInt64(CalculateDirectoryHash(directories[i]));
        }
        /*foreach (var file in Directory.GetFiles(path))
        {
            var fileHash = CalculateFileHash(file);
            result = CombineHashes(checkSum, result, fileHash);
        }

        Array.Sort(directories);*/
        for (int i = 0; i < files.Length; i++)
        {
            result += BitConverter.ToInt64(CalculateFileHash(files[i]));
        }
        
        result += Path.GetDirectoryName(path).Length;

        return BitConverter.GetBytes(result);
    }

    private async Task<byte[]> CalculateFileHashAsync(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return await checkSum.ComputeHashAsync(stream);
    }

    private async Task<byte[]> CalculateDirectoryHashAsync(string path)
    {
        long result = 0;
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);
        var tasks = new Task<byte[]>[directories.Length + files.Length];
        
        Array.Sort(directories);
        
        for (int i = 0; i < directories.Length; i++)
        {
            var index = i;
            tasks[i] = Task.Run(async () => await CalculateDirectoryHashAsync(directories[index]));
        }
        /*foreach (var file in Directory.GetFiles(path))
        {
            var fileHash = CalculateFileHash(file);
            result = CombineHashes(checkSum, result, fileHash);
        }

        Array.Sort(directories);*/
        for (int i = 0; i < files.Length; i++)
        {
            var index = i;
            tasks[directories.Length + index] = Task.Run(async () => await CalculateFileHashAsync(files[index]));
        }

        for (int i = 0; i < tasks.Length; i++)
        {
            result += BitConverter.ToInt64(tasks[i].Result);
        }
        
        result += Path.GetDirectoryName(path).Length;

        return BitConverter.GetBytes(result);
    }
}