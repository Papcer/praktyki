using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using App.Context;
using App.Models;
using Bogus;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Domain.Builders;
using Gotenberg.Sharp.API.Client.Domain.Builders.Faceted;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using SkiaSharp;
using ZXing;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PDFController : ControllerBase
{
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly ApplicationContext _context;
    
    //losowy numer do nazwy biletu
    static Random Rand = new Random(Math.Abs( (int) DateTime.Now.Ticks));

  
    public PDFController(IHostingEnvironment hostingEnvironment, ApplicationContext context)
    {
        _hostingEnvironment = hostingEnvironment;
        _context = context;
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
    public async Task<ActionResult> HtmlToPdf([FromServices] GotenbergSharpClient sharpClient)
    {
        try
        {
            var randomCustomer = await _context.aplikacja_userdata.ToListAsync();
            var randomcustomer = randomCustomer[new Random().Next(randomCustomer.Count)];

            var eve = await _context.aplikacja_event.ToListAsync();
            var ev = eve[new Random().Next(eve.Count)];

            var randomticket = await _context.aplikacja_ticket
                .FirstOrDefaultAsync(t => t.event_id == ev.id);

            var user = await _context.aplikacja_user
                .FirstOrDefaultAsync(u => u.id == randomcustomer.UserId);

            if (randomticket == null || ev == null || randomcustomer == null)
            {
                return BadRequest("Nie udało się pobrać wymaganych danych.");
            }

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

            using (var resulStream = await sharpClient.HtmlToPdfAsync(req))
            {
                if (resulStream != null)
                {
                    string folderPath = Path.Combine(_hostingEnvironment.ContentRootPath, "GeneratedTickets");
                    Directory.CreateDirectory(folderPath);

                    string fileName = $"ticket-{Rand.Next()}.pdf";
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var fileStream = System.IO.File.Create(filePath))
                    {
                        await resulStream.CopyToAsync(fileStream);
                        //return this.File(result, "application/pdf", $"ticket-{Rand.Next()}.pdf");
                    }
                    
                    return Ok("Poprawnie wygenerowano bilety");
                }
                else
                {
                    return BadRequest("Problem podczas konwertowania szablonu html biletu na PDF");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Wystapił problem, {ex.Message}");
        }

        //string htmlFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Templates/index.html");
        //string htmlContent = System.IO.File.ReadAllText(htmlFilePath);
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
