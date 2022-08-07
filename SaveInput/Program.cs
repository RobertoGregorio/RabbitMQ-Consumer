using Microsoft.Extensions.Options;
using SaveInput;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<MongoDatabaseSettings>(hostContext.Configuration.GetSection("MongoDatabase"));

        services.AddSingleton<LogsMongoService>();

    })
    .Build();


await host.RunAsync();
