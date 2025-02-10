// <copyright file="Server.cs" company="NematMusaev">
// Copyright (c) Nemat Musaev. All rights reserved.
// </copyright>

namespace Server;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Dynamic;

/// <summary>
/// class of server.
/// </summary>
public class Server
{
    private readonly CancellationTokenSource cancellationToken = new ();
    private int port;
    private IPAddress ipAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="ipAddress">ipadress server.</param>
    /// <param name="port">port client and server.</param>
    public Server(IPAddress ipAddress, int port)
    {
        this.ipAddress = ipAddress;
        this.port = port;
        Run();
    }

    /// <summary>
    /// Start connecting.
    /// </summary>
    /// <returns>task.</returns>
    public async Task Start()
    {
        await this.Run();
    }

    /// <summary>
    /// executing commands.
    /// </summary>
    /// <returns>task.</returns>
    private async Task Run()
    {
        using var listener = new TcpListener(this.ipAddress, this.port);
        listener.Start();
        while (!this.cancellationToken.Token.IsCancellationRequested)
        {
            using var socket = await listener.AcceptSocketAsync();
            var task = Task.Run(async () =>
            {
                using var stream = new NetworkStream(socket);
                using var reader = new StreamReader(stream);
                var stringCommand = await reader.ReadLineAsync() ?? throw new InvalidOperationException("stringcomand cant be null");
                var command = stringCommand.Split(' ');

                try
                {
                    var path = command[1];
                    if (!(File.Exists(path) || Directory.Exists(path)))
                    {
                        await this.SendError(stream);
                    }

                    int number;
                    int.TryParse(command[0], out number);
                    switch (number)
                    {
                        case 1:
                            await this.List(path, stream);
                            break;
                        case 2:
                            await this.Get(path, stream);
                            break;
                        default:
                            var writer = new StreamWriter(stream);
                            await writer.WriteLineAsync("Command must be either 1 or 2.");
                            break;
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("not file");
                }
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("not directory");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                socket.Close();
            });
        }
    }

    /// <summary>
    /// interrupts working.
    /// </summary>
    public void Stop() => this.cancellationToken.Cancel();

    private async Task SendError(Stream stream)
    {
        using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync($"-1");
        await writer.FlushAsync();
    }

    private async Task List(string path, Stream stream)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException("not directory");
        }

        var directory = new DirectoryInfo(path);
        var files = Directory.GetFileSystemEntries(path);
        Array.Sort(files);
        var builder = new StringBuilder($"{files.Length} ");

        foreach (var file in files)
        {
            builder.Append($"{Path.GetFileName(file)} {Directory.Exists(file).ToString().ToLower()} ");
        }

        using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(builder.ToString()[..^1] + "\n");
        await writer.FlushAsync();
    }

    private async Task Get(string path, Stream stream)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("not file");
        }

        var file = new FileInfo(path);
        var builder = new StringBuilder($"{file.Length} ");
        var info = await File.ReadAllBytesAsync(path);
        using var writer = new StreamWriter(stream);
        await stream.WriteAsync(info);
        await writer.FlushAsync();
    }
}