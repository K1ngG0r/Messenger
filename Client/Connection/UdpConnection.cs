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
        //_udpClientEndPoint = new IPEndPoint(IPAddress.Loopback, port);
        _udpClientEndPoint = new IPEndPoint(IPAddress.Any, port);
        _udpClient = new UdpClient(_udpClientEndPoint);
    }
     public void Start()
    {
        if (_started) return;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        
        _started = true;
        Task.Run(() => ReceiveAsync(_cts.Token));
    }

    public void Stop()
    {
        if (!_started) return;

        _started = false;
        _cts.Cancel();
        return;
    }
    public void Send(byte[] bytes, IPEndPoint endPoint)
    {
        _udpClient.Send(bytes, endPoint);
    }
    private async Task ReceiveAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            UdpReceiveResult result = default;
            try
            {
                result = await _udpClient.ReceiveAsync(cancellationToken);
            }
            catch
            {
                continue;
            }

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