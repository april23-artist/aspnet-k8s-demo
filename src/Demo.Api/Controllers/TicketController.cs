using Demo.Common;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace Demo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ILogger<TicketController> _logger;
        private readonly IDatabase _redis;

        public TicketController(ILogger<TicketController> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis.GetDatabase();
        }

        [HttpPost("order")]
        public async Task<IActionResult> Order([FromBody] TicketRequest request)
        {
            _logger.LogInformation(
                "訂單請求：使用者 {UserId}, 活動 {Event}, 數量 {Quantity}",
                request.UserId,
                request.EventId,
                request.Quantity
            );

            request.RequestTime = DateTime.UtcNow;
            var payload = JsonSerializer.Serialize(request);

            await _redis.ListLeftPushAsync("ticket_queue", payload);

            return Ok(new
            {
                message = "Request queued",
                request.UserId,
                request.EventId
            });
        }
    }
}
