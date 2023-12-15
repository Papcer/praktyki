using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using App.Models;
using App.Services;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Domain.Builders;
using Gotenberg.Sharp.API.Client.Domain.Builders.Faceted;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ZXing;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PDFController : ControllerBase
{
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly MongoDbService _mongoDbService;

    //losowy numer do nazwy biletu
    static Random Rand = new Random(Math.Abs( (int) DateTime.Now.Ticks));

    public PDFController(IHostingEnvironment hostingEnvironment, MongoDbService mongoDbService)
    {
        _hostingEnvironment = hostingEnvironment;
        _mongoDbService = mongoDbService;
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
        ObjectId customerId = ObjectId.Parse(randomCustomer.Id);
        
        var allEvents = await _mongoDbService.GetEventAsync();
        
        //tworznie biletu 
        var ticket = new Ticket
        {
            Id= "asdfasdfasdfasd",
            CustomerId = customerId,
            EventId = ObjectId.GenerateNewId(),
            Status = "Active"
        };
        
        //tworzenie eventu
        var events = new Event
        {
            name = "Wielkie wydarzenie",
            location = "Ulica wielka nowa itp"
        };
        
        //string htmlFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Templates/index.html");
        //string htmlContent = System.IO.File.ReadAllText(htmlFilePath);
        
        string htmlContent = GenerateHtmlContent(ticket, randomCustomer, events);
        
        //string qrCodeContent = GenerateQrCode(ticket.Id);
        
      //  htmlContent = htmlContent.Replace("<!-- INSERT_QR_CODE -->", qrCodeContent);
        
        var builder = new HtmlRequestBuilder()
            .AddDocument(doc =>
                doc.SetBody(htmlContent).SetFooter(htmlContent)
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
    private string GenerateHtmlContent(Ticket ticket, Customer customer, Event events)
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
                <p>Email: {customer.email}   </p>
                <p>Numer telefonu klienta: {customer.phoneNumber}</p>
            </div>

            <div>
                <p>Status biletu: {ticket.Status}</p>
            </div>

            <div>
                <h2>Wydarzenie:</h2>
            </div>
            <div>
                <p>Nazwa wydarzenia: {events.name}</p>
            </div>
            <div>
                <p>Odbedzie sie w : {events.location}</p>
            </div>

            <div>
            <!-- INSERT_QR_CODE_HERE -->
            </div>
        </body>
        </html>
    ";
    }

    //pobieranie losowego klienta z bazy jesli taki istnieje
    private async Task<Customer> GetRandomCustomer()
    {
        var allCustomers = await _mongoDbService.GetCustomerAsync();
        
        if (allCustomers == null || allCustomers.Count == 0)
        {
            return null;
        }
        
        var random = new Random();
        int randomIndex = random.Next(0, allCustomers.Count);
        return allCustomers[randomIndex];
    }
    
    //generowanie kodu QR na podstawie id biletu
    private static string GenerateQrCode(string ticketId)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(ticketId, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new QRCode(qrCodeData);

        Bitmap qrCodeImage = qrCode.GetGraphic(20);

        using (MemoryStream stream = new MemoryStream())
        {
            qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            byte[] qrCodeBytes = stream.ToArray();
            return Convert.ToBase64String(qrCodeBytes);
        }
    }
}
