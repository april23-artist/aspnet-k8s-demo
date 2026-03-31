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
                try
                {
                    var ticket = await _redis.ListRightPopAsync("ticket_queue");
                    if (ticket.IsNullOrEmpty)
                    {
                        await Task.Delay(100); // 避免 busy loop
                        continue;
                    }

                    /* 處理購票 */

                    await Task.Delay(1000, stoppingToken);
                    var request = JsonSerializer.Deserialize<TicketRequest>(ticket!);
                    await ProcessOrder(request!);
                }
                catch (RedisConnectionException ex)
                {
                    // 這裡只紀錄日誌，不拋出異常
                    _logger.LogError("Redis 連不上，等待重試... {Message}", ex.Message);
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private async Task ProcessOrder(TicketRequest request)
        {
            // 模擬 DB / 訂單處理
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
