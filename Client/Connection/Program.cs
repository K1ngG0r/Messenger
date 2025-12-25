using Server;

var CancellationToken = new CancellationToken();

var server = new UdpServer(9000);
await server.StartAsync(CancellationToken);

Console.ReadLine();
await server.StopAsync();