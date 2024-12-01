// <copyright file="SimpleFTP.Tests.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests;

using NetWork;
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
    using var streamclient = client.GetStream();
    using var writer = new StreamWriter(streamclient);
    await writer.WriteLineAsync("1 ./Fileshhh");
    await writer.FlushAsync();

    using var streamserver = new NetworkStream(socket);
    using var reader = new StreamReader(streamserver);
    var response = await reader.ReadLineAsync();
    Assert.That(response, Is.EqualTo(checker));
}

    /// <summary>
    /// testing of wrong command.
    /// </summary>
    /// <returns>task.</returns>
    [Test]
    public async Task GetError()
    {
        string commande = $"1 yyy\n";
        var checker = "-1";
        const int port = 8897;
        var server = new Server(IPAddress.Any, port);
        var client = new Client("localhost", port);
        var result = await client.SendAndAcceptMessage(commande);
        Assert.That(result, Is.EqualTo(checker));
    }

    /// <summary>
    /// testing of list command.
    /// </summary>
    /// <returns>task.</returns>
    [Test]
    public async Task ListDirectory()
    {
        string commande = $"1 ../../../Files";
        var checker = "3 1t.txt false dir true 2t.txt false";
        const int port = 8897;
        var server = new Server(IPAddress.Any, port);
        var client = new Client("localhost", port);
        var result = await client.SendAndAcceptMessage(commande);
        Assert.That(result, Is.EqualTo(checker));
    }
}