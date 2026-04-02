using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Formatting.Json;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName() // 紀錄是哪台機器
    .Enrich.WithThreadId()    // 紀錄執行緒
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 讀取設定檔，如果沒設定就預設 localhost
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    // 加上 abortConnect=false，這樣 Redis 暫時斷線時，程式不會直接掛掉
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(configuration);
});

// 1. 註冊健康檢查服務
builder.Services.AddHealthChecks()
    // 檢查應用程式是否存活
    .AddCheck("self", () => HealthCheckResult.Healthy())
    // 檢查 Redis 連線是否正常
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379", name: "redis");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 2. 設定健康檢查的 Endpoint
// 外部監控看 /healthz
app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    // 只要程式沒死，不檢查 Redis (Liveness)
    Predicate = (check) => check.Name == "self"
});

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    // 所有的檢查都要過，才算 Ready 接收流量 (Readiness)
    Predicate = (_) => true
});

app.Run();
