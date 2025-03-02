namespace NetWork;

using System.Net;
using System.Net.Sockets;

public class Client
{
    private readonly int port;

    private readonly string adress;

    public Client(string adress, int port)
    {
        this.port = port;
        this.adress = adress;
    }

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

        using var client = new TcpClient(adress, port);
        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
        var response = await reader.ReadLineAsync();

        return response;
    }
}