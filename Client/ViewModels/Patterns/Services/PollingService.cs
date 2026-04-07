using Client.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Patterns.Services
{
    public class PollingService
    {
        private ClientConnection _connection;
        private CancellationTokenSource? _pollingCts;
        private readonly int _secondsDelay = 20;
        public event Action<List<SingleChange>>? UpdateReceived;
        public PollingService(ClientConnection connection)
        {
            _connection = connection;
        }

        public void StartUpdatePolling()
        {
            _pollingCts?.Cancel();
            _pollingCts = new CancellationTokenSource();
            Task.Run(() => StartUpdatePollingCycle(_pollingCts.Token, TimeSpan.FromSeconds(_secondsDelay)));
        }
        public void StopUpdatePolling()
        {
            _pollingCts?.Cancel();
        }
        private async Task StartUpdatePollingCycle(CancellationToken token, TimeSpan delay)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(delay, token);
                var changes = await _connection.Update();
                if (changes.IsSuccess)
                    UpdateReceived?.Invoke(changes.Value!);
            }
        }
    }
}
