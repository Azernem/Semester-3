// <copyright file="MD5.cs" company="NematMusaev">
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
/// <param name="path">path.</param>
/// <returns>hash. </returns>
    public byte[] CalculateSequential(string path)
    {
        if (File.Exists(path))
        {
            return this.CalculateFileHash(path);
        }
        else
        {
            return this.CalculateDirectoryHash(path);
        }
    }

    /// <summary>
    /// Parallel Calculate.
    /// </summary>
    /// <param name="path">path.</param>
    /// <returns>hash by multiplication. </returns>
    public async Task<byte[]> CalculateParallel(string path)
    {
        if (File.Exists(path))
        {
            return await this.CalculateFileHashAsync(path);
        }
        else
        {
            return await this.CalculateDirectoryHashAsync(path);
        }
    }

    /// <summary>
    /// Sequentail Calculate hash of file.
    /// </summary>
    /// <param name="path">path. </param>
    /// <returns>hash of file. </returns>
    private byte[] CalculateFileHash(string path)
    {
        var bytes = File.ReadAllBytes(path);
        return this.checkSum.ComputeHash(bytes);
    }

    /// <summary>
    /// Parallel Calculate hash of file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>hash of directory. </returns>
    private byte[] CalculateDirectoryHash(string path)
    {
        long result = Path.GetDirectoryName(path).Length;
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        Array.Sort(directories);

        for (int i = 0; i < directories.Length; i++)
        {
            result += BitConverter.ToInt64(this.CalculateDirectoryHash(directories[i]));
        }

        Array.Sort(files);
        for (int i = 0; i < files.Length; i++)
        {
            result += BitConverter.ToInt64(this.CalculateFileHash(files[i]));
        }

        return BitConverter.GetBytes(result);
    }

    /// <summary>
    /// Parallel Calculate hash of file.
    /// </summary>
    /// <param name="path">path. </param>
    /// <returns>hash of file.</returns>
    private async Task<byte[]> CalculateFileHashAsync(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return await this.checkSum.ComputeHashAsync(stream);
    }

    /// <summary>
    /// Parallel Calculate hash of directory.
    /// </summary>
    /// <param name="path">path. </param>
    /// <returns>hash of directory. </returns>
    private async Task<byte[]> CalculateDirectoryHashAsync(string path)
    {
        long result = Path.GetDirectoryName(path).Length;
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);
        var tasks = new Task<byte[]>[directories.Length + files.Length];

        Array.Sort(directories);

        for (int i = 0; i < directories.Length; i++)
        {
            var locall = i;
            tasks[i] = Task.Run(async () => await this.CalculateDirectoryHashAsync(directories[locall]));
        }

        Array.Sort(files);
        for (int i = 0; i < files.Length; i++)
        {
            var locall = i;
            tasks[directories.Length + locall] = Task.Run(async () => await this.CalculateFileHashAsync(files[locall]));
        }

        for (int i = 0; i < tasks.Length; i++)
        {
            result += BitConverter.ToInt64(tasks[i].Result);
        }

        return BitConverter.GetBytes(result);
    }
}