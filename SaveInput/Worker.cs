using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SaveInput
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly LogsMongoService _logsMongoService;

        public Worker(ILogger<Worker> logger, LogsMongoService logsMongoService)
        {
            _logger = logger;
            _logsMongoService = logsMongoService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(5000, stoppingToken);

                try
                {

                    var factory = new ConnectionFactory()
                    {
                        HostName = "localhost"
                    };

                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue: "Logs_apis",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            var consumer = new EventingBasicConsumer(channel);

                            consumer.Received += (model, ea) =>
                            {
                                SaveMessage(ea, _logsMongoService, _logger);
                            };


                            channel.BasicConsume(queue: "Logs_apis", autoAck: true, consumer: consumer);

                        }

                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void SaveMessage(BasicDeliverEventArgs ea, LogsMongoService _logsMongoService, ILogger _logger)
        {

            var body = ea.Body.ToArray();

            if (body.Length == 0)
            {
                Console.WriteLine("Fila vazia");
                return;
            }
             

            var logString = Encoding.UTF8.GetString(body);
            Console.WriteLine(logString);

            Log log = JsonConvert.DeserializeObject<Log>(logString);

            _logsMongoService.CreateAsync(log);
        }
    }
}