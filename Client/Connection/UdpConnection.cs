using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Client.Connection;

namespace Client.Connection;

public class UdpConnection
{
    private readonly IPEndPoint _udpClientEndPoint;
    private readonly UdpClient _udpClient;

    private IPresentationService _ps;
    private bool _started = false;
    private CancellationTokenSource _cts = new CancellationTokenSource();
    public Action<string> DataReceived = null!;

    public UdpConnection(int port, IPresentationService ps)
    {
        //_ps = ps;
        //_udpClientEndPoint = new IPEndPoint(IPAddress.Loopback, port);
        //_udpClient = new UdpClient(_udpClientEndPoint);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_started) return;

        _cts = new CancellationTokenSource();
        var linketToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cts.Token);
        _started = true;
        await ReceiveAsync(linketToken.Token);
    }

    public void Stop()
    {
        if (!_started) return;

        _started = false;
        _cts.Cancel();
        _ps.DisplayMessage("Сервер остановлен");
        return;
    }
    public async Task SendAsync(string message, IPEndPoint endPoint)
    {
        await _udpClient.SendAsync(Encoding.UTF8.GetBytes(message), endPoint);
    }
    private async Task ReceiveAsync(CancellationToken cancellationToken)
    {
        _ps.DisplayMessage($"Сервер запущен: {_udpClient.Client.LocalEndPoint}");
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await _udpClient.ReceiveAsync(cancellationToken);

            var remoteEndPoint = result.RemoteEndPoint;
            var requestBytes = result.Buffer;
            _ps.DisplayMessage($"Входящее подключение {remoteEndPoint}");

            ClientHanlde(remoteEndPoint, requestBytes);
        }
    }

    private void ClientHanlde(IPEndPoint remoteEndPoint, byte[] requestBytes)
    {
        var requestString = Encoding.UTF8.GetString(requestBytes);
        _ps.DisplayMessage(requestString);
        DataReceived?.Invoke(requestString);
    }
}