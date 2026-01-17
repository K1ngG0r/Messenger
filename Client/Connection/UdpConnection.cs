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

    private bool _started = false;
    private CancellationTokenSource _cts = new CancellationTokenSource();
    public Action<byte[], IPEndPoint> DataReceived = null!;

    public UdpConnection(int port)
    {
        _udpClientEndPoint = new IPEndPoint(IPAddress.Loopback, port);
        _udpClient = new UdpClient(_udpClientEndPoint);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_started) return;

        _cts?.Cancel();
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
        return;
    }
    public async Task SendAsync(byte[] bytes, IPEndPoint endPoint)
    {
        await _udpClient.SendAsync(bytes, endPoint);
    }
    private async Task ReceiveAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await _udpClient.ReceiveAsync(cancellationToken);

            var remoteEndPoint = result.RemoteEndPoint;
            var requestBytes = result.Buffer;

            ClientHanlde(remoteEndPoint, requestBytes);
        }
    }

    private void ClientHanlde(IPEndPoint remoteEndPoint, byte[] requestBytes)
    {
        DataReceived?.Invoke(requestBytes, remoteEndPoint);
    }
}