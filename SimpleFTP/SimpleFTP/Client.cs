// <copyright file="Client.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace NetWork;

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
    /// sending ans accepting message for client.
    /// </summary>
    /// <param name="message">message to server.</param>
    /// <returns>response of server.</returns>
    /// <exception cref="ArgumentException">Argument exception.</exception>
    public async Task<string> SendAndAcceptMessage(string message)
    {
        var array = message.Split(' ');

        if (array.Length == 0)
        {
            throw new ArgumentException("This is not argument \nEnter the commande for server and directory, file ");
        }
        else if (array.Length == 1)
        {
            throw new ArgumentException("Please, enter the directory or file");
        }
        else if (array.Length > 2)
        {
            throw new ArgumentException("the lot of parameters");
        }

        using var client = new TcpClient(this.address, this.port);
        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
        var response = await reader.ReadLineAsync();

        return response ?? throw new InvalidOperationException("No response received from the server.");
    }
}