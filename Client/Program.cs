using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Shared;

var udpClient = new UdpClient();
var serverEndPoint = new IPEndPoint(IPAddress.Loopback, 9000);
udpClient.Connect(serverEndPoint);
Console.WriteLine($"Клиент запущен: {udpClient.Client.LocalEndPoint}");

var request = new Request(RequestMethod.Time, string.Empty);
var requestString = JsonSerializer.Serialize(request);
var requestBytes = Encoding.UTF8.GetBytes(requestString);
await udpClient.SendAsync(requestBytes);
Console.WriteLine("Запрос отправлен");

var result = await udpClient.ReceiveAsync();
var responseString = Encoding.UTF8.GetString(result.Buffer);
var response = JsonSerializer.Deserialize<Response>(responseString);

if (response.Code == ResponseStatusCode.Ok)
{
    Console.WriteLine(response.Body);
}
else
{
    Console.WriteLine($"Ошибка на сервере: {response.Body}");
}
Console.ReadLine();