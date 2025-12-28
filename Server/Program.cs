
namespace Server;

internal class Program
{
    static async Task Main(string[] args)
    {
        var CancellationToken = new CancellationToken();

        var server = new UdpServer(9000);
        await server.StartAsync(CancellationToken);

        Console.ReadLine();
        await server.StopAsync();
    }
}
