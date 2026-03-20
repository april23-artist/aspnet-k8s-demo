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
        private readonly IDatabase _redis;

        public TicketController(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        [HttpPost("order")]
        public async Task<IActionResult> Order([FromBody] TicketRequest request)
        {
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
