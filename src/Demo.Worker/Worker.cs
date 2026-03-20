using Demo.Common;
using StackExchange.Redis;
using System.Text.Json;

namespace Demo.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDatabase _redis;

        public Worker(ILogger<Worker> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis.GetDatabase();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await _redis.ListRightPopAsync("ticket_queue");

                if (result.IsNullOrEmpty)
                {
                    await Task.Delay(100); // Á×§K busy loop
                    continue;
                }

                var request = JsonSerializer.Deserialize<TicketRequest>(result!);

                await ProcessOrder(request!);
            }
        }

        private async Task ProcessOrder(TicketRequest request)
        {
            // ¼ÒÀÀ DB / ­q³æ³B²z
            await Task.Delay(50);

            _logger.LogInformation(
                "Processed order: User={UserId}, Event={EventId}, Qty={Qty}",
                request.UserId,
                request.EventId,
                request.Quantity
            );
        }
    }
}
