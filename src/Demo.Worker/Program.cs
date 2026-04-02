using Demo.Worker;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Formatting.Json;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName() // 紀錄是哪台機器
    .Enrich.WithThreadId()    // 紀錄執行緒
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

// 讀取設定檔，如果沒設定就預設 localhost
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    // 加上 abortConnect=false，這樣 Redis 暫時斷線時，程式不會直接掛掉
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddHostedService<Worker>();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379");

var app = builder.Build();

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = (check) => check.Name == "self"
});

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = (_) => true
});

app.Run();