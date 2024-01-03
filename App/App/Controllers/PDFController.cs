using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using App.Context;
using App.Models;
using App.Services;
using Bogus;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Domain.Builders;
using Gotenberg.Sharp.API.Client.Domain.Builders.Faceted;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using SkiaSharp;
using StackExchange.Redis;
using ZXing;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Ticket = App.Models.Ticket;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PDFController : ControllerBase
{
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly ApplicationContext _context;
    private readonly RedisService _redisService;
    
    //losowy numer do nazwy biletu
    static Random Rand = new Random(Math.Abs( (int) DateTime.Now.Ticks));

  
    public PDFController(IHostingEnvironment hostingEnvironment, ApplicationContext context, RedisService redisService)
    {
        _hostingEnvironment = hostingEnvironment;
        _context = context;
        _redisService = redisService;
    }
    
    /// <summary>
    /// Api korzystające z narzędzia Gotenberg do konwertowania pliku html na pdf.
    /// Pobiera z bazy losowy event, klienta oraz utworzony wcześniej bilet i generuje plik pdf
    /// </summary>
    /// <returns>
    /// Bilet w formie pdf zawierający informację o wydarzeniu, kliencie
    /// </returns>
    /// <response code="200">Plik biletu został wygenerowany</response>
    /// <response code="400">Bład podczas generowania</response>
    [HttpGet("ConvertHtmlToPdf")]
    public async Task<ActionResult> HtmlToPdf([FromServices] GotenbergSharpClient sharpClient, int? ticketID)
    {
        var randomticket = new Ticket();
        var customer = new Customer();
        var even = new Event();
        var user = new User();

        if (ticketID != null)
        {
            string redisKey = $"TicketData:{ticketID}";
            byte[] pdfBytes = await _redisService.GetAsync(redisKey);
            if (pdfBytes != null)
            {
                return File(pdfBytes, "application/pdf", $"ticket-{ticketID}.pdf");
            }
            else
            {

                randomticket = await _context.aplikacja_ticket
                    .FirstOrDefaultAsync(t => t.id == ticketID);

                customer = await _context.aplikacja_userdata.FirstOrDefaultAsync(u =>
                    u.UserId == randomticket.userdata_id);

                user = await _context.aplikacja_user
                    .FirstOrDefaultAsync(u => u.id == customer.UserId);

                even = await _context.aplikacja_event.FirstOrDefaultAsync(a =>
                    a.id == randomticket.event_id);

                string qrCodeContent = GenerateQrCode(randomticket.id.ToString());
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

                using (var resultStream = await sharpClient.HtmlToPdfAsync(req))
                {
                    if (resultStream != null)
                    {
                        // Zapisz plik PDF do Redis
                        byte[] pdfBytess = await ToByteArrayAsync(resultStream);
                        await _redisService.SetAsync(redisKey, pdfBytess, TimeSpan.FromHours(12));
                        return File(pdfBytess, "application/pdf", $"ticket-{randomticket.id}.pdf");
                    }
                    else
                    {
                        return BadRequest("Problem podczas konwertowania szablonu html biletu na PDF");
                    }
                }
            }
        }

        try
        {
            var randomCustomer = await _context.aplikacja_userdata.ToListAsync();
            var randomcustomer = randomCustomer[new Random().Next(randomCustomer.Count)];

            var eve = await _context.aplikacja_event.ToListAsync();
            var ev = eve[new Random().Next(eve.Count)];

            if (ticketID != null)
            {
                randomticket = await _context.aplikacja_ticket
                    .FirstOrDefaultAsync(t => t.id == ticketID);
            }
            else
            {
                randomticket = await _context.aplikacja_ticket
                .FirstOrDefaultAsync(t => t.event_id == ev.id);
            }


            user = await _context.aplikacja_user
                .FirstOrDefaultAsync(u => u.id == randomcustomer.UserId);

            if (randomticket == null || ev == null || randomcustomer == null)
            {
                return BadRequest("Nie udało się pobrać wymaganych danych.");
            }

            string redisKey = $"TicketData:{randomticket.id}";
            byte[] pdfBytes = await _redisService.GetAsync(redisKey);

            //jesli nie ma wpisu w redis, generuj dokument
            if (pdfBytes == null)
            {
                string qrCodeContent = GenerateQrCode(randomticket.id.ToString());
                string htmlContent = GenerateHtmlContent(randomticket, randomcustomer, user, ev, qrCodeContent);

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

                using (var resultStream = await sharpClient.HtmlToPdfAsync(req))
                {
                    if (resultStream != null)
                    {
                        // Zapisz plik PDF do Redis
                        byte[] pdfBytess = await ToByteArrayAsync(resultStream);
                        await _redisService.SetAsync(redisKey, pdfBytess, TimeSpan.FromHours(12));
                        return File(pdfBytess, "application/pdf", $"ticket-{randomticket.id}.pdf");
                       
                        /*
                        string folderPath = Path.Combine(_hostingEnvironment.ContentRootPath, "GeneratedTickets");
                        Directory.CreateDirectory(folderPath);

                        string fileName = $"ticket-{Rand.Next()}.pdf";
                        string filePath = Path.Combine(folderPath, fileName);

                        using (var fileStream = System.IO.File.Create(filePath))
                        {
                            await resultStream.CopyToAsync(fileStream);
                            
                        }  */
                    }
                    else
                    {
                        return BadRequest("Problem podczas konwertowania szablonu html biletu na PDF");
                    }
                }
            }
            else
            {
                // Jeśli plik PDF jest już w Redis, zwróć go bez ponownej konwersji
                return File(pdfBytes, "application/pdf", $"ticket-{randomticket.id}.pdf");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Wystąpił problem: {ex.Message}");
        }
    }

    public static async Task<byte[]> ToByteArrayAsync(Stream stream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
    
    //generowanie szablonu html biletu
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
    
    //generowanie kodu QR na podstawie id biletu
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
    
}
