using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using App.Context;
using App.Models;
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
    /// Api korzystające z narzędzia Gotenberg do konwertowania pliku html na pdf
    /// </summary>
    /// <returns>
    /// Bilet w formie pdf zawierający informację o wydarzeniu, kliencie
    /// </returns>
    /// <response code="200">Plik biletu został wygenerowany</response>
    /// <response code="400">Bład podczas generowania</response>
    [HttpGet("ConvertHtmlToPdf")]
    public async Task<ActionResult> HtmlToPdf([FromServices] GotenbergSharpClient sharpClient)
    {
        //wybranie losowego klienta z bazy
        Customer randomCustomer = await GetRandomCustomer();
        
        //tworznie biletu 
        var ticket = new Ticket
        {
            id= 1,
            event_id = 1,
            userdata_id = 1,
        };
        
        //tworzenie eventu
        var events = new Event
        {
            title = "Wielkie wydarzenie",
            location = "Ulica wielka nowa itp"
        };
        
        //string htmlFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Templates/index.html");
        //string htmlContent = System.IO.File.ReadAllText(htmlFilePath);
        
        string qrCodeContent = GenerateQrCode(ticket.id.ToString());
        string htmlContent = GenerateHtmlContent(ticket, randomCustomer, events, qrCodeContent);
        
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

        var result = await sharpClient.HtmlToPdfAsync(req);

        return this.File(result, "application/pdf", $"ticket-{Rand.Next()}.pdf");
    }
    
    //generowanie szablonu html biletu
    private string GenerateHtmlContent(Ticket ticket, Customer customer, Event events, string qrCodeContent)
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
                <p>Email: {customer.Email}   </p>
                <p>Numer telefonu klienta: {customer.phonenumber}   </p>
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
                <img src='{qrCodeContent}' width='150' height='150' />
            </div>
        </body>
        </html>
    ";
    }

    //pobieranie losowego klienta z bazy jesli taki istnieje
    private async Task<Customer> GetRandomCustomer()
    {
        var allCustomers = await _context.aplikacja_userdata
            .Include(c => c.User) 
            .ToListAsync();
        
        if (allCustomers == null || allCustomers.Count == 0)
        {
            return null;
        }
        
        var random = new Random();
        int randomIndex = random.Next(0, allCustomers.Count);
        return allCustomers[randomIndex];
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
