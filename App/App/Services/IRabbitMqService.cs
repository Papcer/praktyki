namespace App.Services;

public interface IRabbitMqService
{
    public Task SubscribeToQueue();
    //public Task PublishPdfGenerationMessageAsync(int? ticketId);
    public void PublishPdfGenerationMessageAsync(int? ticketId, string userId);

}