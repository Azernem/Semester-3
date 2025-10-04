// <copyright file="SimpleFTP.Tests.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests;

using Server;
using Client;
using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// class of tests.
/// </summary>
public class Tests
{
    /// <summary>
    /// testing of get commande.
    /// </summary>
    /// <returns>task.</returns>
    [Test]
    public async Task GetResponseForClient()
    {
        const string checker = "1 ./Fileshhh";
        const int port = 8896;

        using var server = new TcpListener(IPAddress.Any, port);
        server.Start();

        using var client = new TcpClient("localhost", port);
        using var socket = await server.AcceptSocketAsync();
        using var streamClient = client.GetStream();
        using var writer = new StreamWriter(streamClient);
        await writer.WriteLineAsync("1 ./Fileshhh");
        await writer.FlushAsync();

        using var streamserver = new NetworkStream(socket);
        using var reader = new StreamReader(streamserver);
        var response = await reader.ReadLineAsync();
        Assert.That(response, Is.EqualTo(checker));
    }

    /// <summary>
    /// At Get method file about path not found.
    /// </summary>
    /// <returns>task.</returns>
    [Test]
    public async Task GetError()
    {
        string commande = $"yyy\n";
        var checker = "-1";
        const int port = 8897;
        var server = new Server(IPAddress.Any, port);
        var client = new Client("localhost", port);
        var result = await client.Get(commande);
        Assert.That(result, Is.EqualTo(checker));
    }

    /// <summary>
    /// Get File Data.
    /// </summary>
    /// <returns>task.</returns>
    [Test]
    public async Task GetFileData()
    {
        string path = "../../../Files/1t.txt";
        const int port = 8897;
        var checkData = await File.ReadAllBytesAsync(path);
        var server = new Server(IPAddress.Any, port);
        var client = new Client("localhost", port);
        /*Assert.That(client.Get(path), Is.EqualTo(checkData));*/
    }

    /// <summary>
    /// testing of list command.
    /// </summary>
    /// <returns>task.</returns>
    [Test]
    public async Task ListDirectory()
    {
        string commande = $"../../../Files";
        var checker = "3 1t.txt false 2t.txt false dir true";
        const int port = 8897;
        var server = new Server(IPAddress.Any, port);
        var client = new Client("localhost", port);
        var result = await client.List(commande);
        Assert.That(result, Is.EqualTo(checker));
    }

    /// <summary>
    /// At List method file about path not found.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task ListError()
    {
        string commande = $"yyy\n";
        var checker = "-1";
        const int port = 8897;
        var server = new Server(IPAddress.Any, port);
        var client = new Client("localhost", port);
        var result = await client.List(commande);
        Assert.That(result, Is.EqualTo(checker));
    }
}