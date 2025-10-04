// <copyright file="Client.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Client;

using Server;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Client class.
/// </summary>
public class Client
{
    private readonly int port;
    private readonly string address;

    /// <summary>
    /// constructor.
    /// </summary>
    /// <param name="address">ip address.</param>
    /// <param name="port">port.</param>
    public Client(string address, int port)
    {
        this.port = port;
        this.address = address;
    }

    /// <summary>
    /// Method List of client. List this directory.
    /// </summary>
    /// <param name="message">path.</param>
    /// <returns>server response.</returns>
    /// <exception cref="InvalidOperationException">invalid operation.</exception>
    public async Task<string> List(string message)
    {
        using var client = new TcpClient(this.address, this.port);
        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);
        await writer.WriteLineAsync($"1 {message}");
        await writer.FlushAsync();
        var response = await reader.ReadLineAsync();

        return response ?? throw new InvalidOperationException("No response received from the server.");
    }

    /// <summary>
    /// Mrthod Get of client. Get file data.
    /// </summary>
    /// <param name="message">path.</param>
    /// <returns>reserver response.</returns>
    /// <exception cref="InvalidOperationException">Invalid operation.</exception>
    public async Task<byte[]> Get(string message)
    {
        using var client = new TcpClient(this.address, this.port);
        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);
        await writer.WriteLineAsync($"2 {message}");
        await writer.FlushAsync();
        byte[] buffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        byte[] response = buffer.Take(bytesRead).ToArray();

        return response ?? throw new InvalidOperationException("No response received from the server.");
    }
}