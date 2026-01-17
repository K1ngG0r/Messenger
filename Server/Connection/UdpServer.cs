using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Shared;

namespace Server;

public class UdpServer
{
    private readonly IPEndPoint _serverEndPoint;
    private readonly UdpClient _udpClient;
    private readonly MessageHandler messageHandler = new MessageHandler();

    private bool _started = false;
    private CancellationTokenSource _cts = new CancellationTokenSource();

    public UdpServer()
    {
        _serverEndPoint = new IPEndPoint(IPAddress.Loopback, 90000);
        _udpClient = new UdpClient(_serverEndPoint);
    }

    public UdpServer(IPAddress iPAddress ,int port)
    {
        _serverEndPoint = new IPEndPoint(iPAddress, port);
        _udpClient = new UdpClient(_serverEndPoint);
    }

     public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_started) return;

        _cts = new CancellationTokenSource();
        var linketToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cts.Token);
        _started = true;
        await ReceiveAsync(linketToken.Token);
    }

    public Task StopAsync()
    {
        if (!_started) return Task.CompletedTask;

        _started = false;
        _cts.Cancel();
        Console.WriteLine("Сервер остановлен");
        return Task.CompletedTask;
    }

    private async Task ReceiveAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Сервер запущен: {_udpClient.Client.LocalEndPoint}");
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await _udpClient.ReceiveAsync(cancellationToken);

            var remoteEndPoint = result.RemoteEndPoint;
            var requestBytes = result.Buffer;
            Console.WriteLine($"Входящее подключение {remoteEndPoint}");

            _ = ClientHanlderAsync(remoteEndPoint, requestBytes, cancellationToken);
        }
    }

    private async Task ClientHanlderAsync(IPEndPoint remoteEndPoint, byte[] requestBytes, CancellationToken cancellationToken)
    {
        string response = messageHandler.RequestHandler(requestBytes, cancellationToken);

        var responseBytes = Encoding.UTF8.GetBytes(response);

        await _udpClient.SendAsync(responseBytes, remoteEndPoint, cancellationToken);
    }
}