using System.Text;
using System.Text.Json;
using App.Class;
using EasyNetQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace App.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly PdfGenerationConsumer _pdfGenerationConsumer;
    private readonly IServiceProvider _serviceProvider;
    

    public RabbitMqService(IConfiguration configuration, PdfGenerationConsumer pdfGenerationConsumer, IServiceProvider serviceProvider , IConnection connection, 
        IModel channel )
    {
        _connection = connection;
        _channel = channel;
        _pdfGenerationConsumer = pdfGenerationConsumer;
        _serviceProvider = serviceProvider;
    }
    public void PublishPdfGenerationMessageAsync(int? ticketId, string userId)
    {
        var messageData = new { TicketId = ticketId, UserId = userId };
        var messageJson = JsonConvert.SerializeObject(messageData);
        var message = Encoding.UTF8.GetBytes(messageJson);
        
        _channel.BasicPublish(exchange: "", routingKey: "pdf", basicProperties: null, body: message);
        
        Console.WriteLine($"PDF generation message published for TicketId: {ticketId} and UserId: {userId}");
    }
    
    public async Task SubscribeToQueue()
    {
        var queueName = "pdf";
        _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);

            try
            {
                var messageData = JsonConvert.DeserializeObject<PdfGenerationMessage>(messageJson);

                // Przekazuje wiadomość do konsumenta PdfGenerationConsumer
                await _pdfGenerationConsumer.ConsumeAsync(messageData.TicketId, messageData.UserId);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        Console.WriteLine($"Subscribed to RabbitMQ queue: {queueName}");
    }
}

