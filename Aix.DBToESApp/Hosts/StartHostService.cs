using Aix.DBToES;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aix.DBToESApp.Hosts
{
    public class StartHostedService : IHostedService
    {
        private SyncToES _syncToES;
        public StartHostedService(SyncToES syncToES)
        {
            _syncToES = syncToES;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                await _syncToES.Sync();

            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
