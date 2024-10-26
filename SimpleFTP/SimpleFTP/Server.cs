namespace NetWork;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Dynamic;

/// <summary>
/// calss of server. 
/// </summary>
public class Server
{
    public string answer;
    private int port;
    private IPAddress ipaddress;
    public Server(IPAddress ipaddress, int port)
    {
        this.ipaddress = ipaddress;
        this.port = port;
        Run();
    }

    public async Task Run()
    {
        using var listener = new TcpListener(this.ipaddress, this.port);
        listener.Start();
        while (true)
        {
            using var socket = await listener.AcceptSocketAsync();
            await Task.Run(async () =>
            {
                using var stream = new NetworkStream(socket);
                using var reader = new StreamReader(stream);
                string stringcommande = await reader.ReadLineAsync();
                this.answer = stringcommande;
                var commande = stringcommande.Split(' ');

                try
                {
                    var path = commande[1]; ///
                    if (!(File.Exists(path) || Directory.Exists(path)))
                    {
                        await SendError(stream);
                    }
                    int n;
                    int.TryParse(commande[0], out n);
                    if (n == 1)
                    {
                        await this.List(path, stream);
                    }
                    else if (n == 2)
                    {
                        await this.Get(path, stream);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                catch (FileNotFoundException ex)
                {
                    Console.WriteLine("not file");
                }

                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine("not directory");
                }
            });
        }
    }

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
                throw new DirectoryNotFoundException("not directory");
            }
            var file = new FileInfo(path);
            var builder = new StringBuilder($"{file.Length}");
            var info = File.ReadAllBytes(path);
            using var writer = new StreamWriter(stream);
            builder.Append($"{file.Length} {Encoding.UTF8.GetString(info).ToString()}");
            writer.Write(builder);
            await writer.FlushAsync();   ///
    }
}