namespace Tests;

using NetWork;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Tests
{
    [Test]
    public async Task GetResponseForClient1()
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
}