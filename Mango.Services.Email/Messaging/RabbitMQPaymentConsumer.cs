using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {

        private IConnection _connection;
        private IModel _channel;
        private const string ExchangeName= "DirectPaymentUpdate_Exchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
        private readonly EmailRepository _emailRepo;
        string queueName = "";
        public RabbitMQPaymentConsumer(EmailRepository emailRepo)
        {
            _emailRepo = emailRepo;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "",
                Password = ""
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            _channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, "PaymentEmail");
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
                HandleMessage(updatePaymentResultMessage).GetAwaiter().GetResult();

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume(PaymentEmailUpdateQueueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
        {
            try
            {
                await _emailRepo.SendAndLogEmail(updatePaymentResultMessage);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
