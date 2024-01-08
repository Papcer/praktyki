using System.Text;
using App.Context;
using App.Controllers;
using App.Models;
using App.Services;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Domain.Builders;
using Gotenberg.Sharp.API.Client.Domain.Builders.Faceted;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QRCoder;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.AspNetCore.Authorization;

namespace App.Class;

public class PdfGenerationConsumer
{
    private readonly GotenbergSharpClient _sharpClient;
    private readonly RedisService _redisService;
    private readonly ApplicationContext _context;
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<PdfHub> _hubContext;

    
    public PdfGenerationConsumer(GotenbergSharpClient sharpClient, RedisService redisService, ApplicationContext context,
        IServiceProvider serviceProvider, IServiceScopeFactory scopeFactory, IHubContext<PdfHub> hubContext, IConnection connection)
    {
        _sharpClient = sharpClient;
        _redisService = redisService;
        _context = context;
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
        _connection = connection;
    }
    
    public async Task ConsumeAsync(int? ticketId, string userId)
    {
        try
        {
            Console.WriteLine($"Consumer started processing message for TicketId: {ticketId}");


            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                var randomticket = await context.aplikacja_ticket
                    .FirstOrDefaultAsync(t => t.id == ticketId);

                var customer = await context.aplikacja_userdata.FirstOrDefaultAsync(u =>
                    u.UserId == randomticket.userdata_id);

                var user = await context.aplikacja_user
                    .FirstOrDefaultAsync(u => u.id == customer.UserId);

                var even = await context.aplikacja_event.FirstOrDefaultAsync(a =>
                    a.id == randomticket.event_id);

                // Generate PDF using GotenbergSharpClient
                string qrCodeContent = GenerateQrCode(ticketId.ToString());
                string htmlContent = GenerateHtmlContent(randomticket, customer, user, even, qrCodeContent);

                var builder = new HtmlRequestBuilder()
                    .AddDocument(doc =>
                        doc.SetBody(htmlContent)
                    ).WithDimensions(dims =>
                    {
                        dims.SetPaperSize(PaperSizes.A3)
                            .SetMargins(Margins.None)
                            .SetScale(.99);
                    });

                var req = await builder.BuildAsync();

                using (var resultStream = await _sharpClient.HtmlToPdfAsync(req))
                {
                    if (resultStream != null)
                    {
                        // Save the PDF to Redis
                        byte[] pdfBytes = await ToByteArrayAsync(resultStream);
                        string redisKey = $"TicketData:{ticketId}";
                        await _redisService.SetAsync(redisKey, pdfBytes, TimeSpan.FromHours(12));
                        
                        var pdfFileInfo = new
                        {
                            TicketId = ticketId,
                            FilePath = $"http://localhost:8080/api/PDF/DownloadPdf/{ticketId}" ,
                            userId = userId
                        };
                        
                        await _hubContext.Clients.Group(userId).SendAsync("NotifyPdfReady" , pdfFileInfo);
                        Console.WriteLine($"PDF generation completed for TicketId: {ticketId}");
                    }
                    else
                    {
                        // Log an error or take appropriate action
                        Console.WriteLine($"PDF generation failed for TicketId: {ticketId}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex.Message}");
        }
    }
    
    private string GenerateQrCode(string data)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
            //tworzenie danych przechowywanych przez kod QR
            QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode($"Numer biletu: {data}", QRCodeGenerator.ECCLevel.Q);
            //tworzenie reprezetnacji kodu w postaci png jako tablica bajtow
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            //tablica bajtow, qr w rozmiarze 20x20 
            byte[] qrCodeBytes = qrCode.GetGraphic(20);
            //zapisanie bajtow do strumienia
            stream.Write(qrCodeBytes, 0, qrCodeBytes.Length);

            // Konwertuj MemoryStream na base64
            return $"data:image/png;base64,{Convert.ToBase64String(stream.ToArray())}";
        }
    }

    private string GenerateHtmlContent(Ticket ticket, Customer customer, User user, Event events, string qrCodeContent)
    {
        return $@"
           <!DOCTYPE html>
        <html>
        <head>
            <title>Bilet</title>
        </head>
        <style>
        body{{
            display:flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
        }}
        div{{
            display: flex;
            
        }}
        </style>

        <body>
            <div>
                <h1>Bilet nr: </h2>
            </div>

            <div>
                <p>Imie klienta: {customer.firstName}   </p><br>
                <p>Nazwisko klienta: {customer.lastName}  </p>
            </div>

            <div>
                <p>Email: {user.username}   </p>
                <p>Numer telefonu klienta: {customer.phoneNumber}   </p>
            </div>


            <div>
                <h2>Wydarzenie:</h2>
            </div>
            <div>
                <p>Nazwa wydarzenia: {events.title}   </p>
            </div>
            <div>
                <p>Odbedzie sie w : {events.location}   </p>
            </div>

            <div>
                <p>Dnia : {events.start_date}   </p>
            </div>

           <div>
                <img src='{qrCodeContent}' width='150' height='150' />
            </div>
        </body>
        </html>
    ";
    }

    public static async Task<byte[]> ToByteArrayAsync(Stream stream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
    

}