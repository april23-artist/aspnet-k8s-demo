using Demo.Worker;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

// 讀取設定檔，如果沒設定就預設 localhost
var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    // 加上 abortConnect=false，這樣 Redis 暫時斷線時，程式不會直接掛掉
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
