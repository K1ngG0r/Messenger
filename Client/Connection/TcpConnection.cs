using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class TcpConnection
{
    public Action<string, EndPoint> DataReceived = null!;
    private bool _started = false;
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private TcpListener listener;
    public TcpConnection(int port)
    {
        listener = new TcpListener(IPAddress.Loopback, port);
    }
    public void Start()
    {
        if (_started == true)
            return;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _started = true;

        Task.Run(() => ReceiveAsync(_cts.Token));
    }
    public void Send(string data)
    {
        listener.Server.Send(Encoding.UTF8.GetBytes(data));
    }
    private async Task ReceiveAsync(CancellationToken cancellationToken)
    {
        listener.Start();
        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);
        }
    }
    private async Task HandleClientAsync(TcpClient client)
    {
        using (var stream = client.GetStream())
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
        {
            try
            {
                // Чтение данных от клиента
                string receivedData = await reader.ReadLineAsync();
                DataReceived?.Invoke(receivedData, client.Client.RemoteEndPoint);

                /*//Отправка ответа клиенту
                await writer.WriteLineAsync("Данные получены сервером!");*/
            }
            catch (IOException)
            {
            }
            finally
            {
                client.Close();
            }
        }
    }
}