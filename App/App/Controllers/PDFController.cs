using System.Security.Claims;
using App.Class;
using App.Context;
using App.Services;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PDFController : ControllerBase
{
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly ApplicationContext _context;
    private readonly RedisService _redisService;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IHubContext<PdfHub> _hubContext;
    private readonly IWebHostEnvironment _env;
    
    public PDFController(IRabbitMqService rabbitMqService ,IHostingEnvironment hostingEnvironment, 
        ApplicationContext context, RedisService redisService , IHubContext<PdfHub> hubContext, IWebHostEnvironment env
       )
    {
        _hostingEnvironment = hostingEnvironment;
        _context = context;
        _redisService = redisService;
        _rabbitMqService = rabbitMqService;
        _hubContext = hubContext;
        _env = env;
    }
    
    private static readonly object _lockObject = new object();

    /// <summary>
    /// Api korzystające z narzędzia Gotenberg do konwertowania pliku html na pdf.
    /// Po podaniu id biletu, sprawdzane jest czy bilet znajduje się w pamięci redis, jesli nie to wysyłana jest wiadomość do kolejki Rabbit, a następnie wywołana
    /// funkcja do generowania biletu, która również zapisze ten bilet do pamięci w Redis'ie
    /// </summary>
    /// <returns>
    /// Bilet w formie pdf zawierający informację o wydarzeniu, kliencie
    /// </returns>
    /// <response code="200">Plik biletu został wygenerowany</response>
    /// <response code="400">Bład podczas generowania</response>
    [Authorize]
    [HttpGet("ConvertHtmlToPdf")]
    public async Task<ActionResult> HtmlToPdf(int? ticketID)
    {
        var userId = GetUserIdFromToken();
        try
        {
            if (ticketID != null)
            {
                string redisKey = $"TicketData:{ticketID}";
                byte[] pdfBytes = await _redisService.GetAsync(redisKey);
                if (pdfBytes != null)
                {
                    
                    var pdfFileInfo = new
                    {
                        TicketId = ticketID,
                        FilePath = $"http://localhost:8080/api/PDF/DownloadPdf/{ticketID}",
                        userId = userId,
                    };

                    await _hubContext.Clients.Group(userId).SendAsync("NotifyPdfReady" , pdfFileInfo);
                    
                    return File(pdfBytes, "application/pdf", $"ticket-{ticketID}.pdf");

                }
                
                _rabbitMqService.PublishPdfGenerationMessageAsync(ticketID, userId);
                return Ok("Generowanie dokumentu w toku...");
            }
            else
            {
                return BadRequest("Błędny numer ID");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Api służące do pobrania pliku pdf, który został wygenerowany. Zostaje wywołane przez klienta, który otrzymuje link do tego endpointu po wygenerowaniu pliku.
    /// </summary>
    /// <returns>
    /// Bilet w formie pdf zawierający informację o wydarzeniu, kliencie
    /// </returns>
    /// <response code="200">Plik biletu został pobr7any</response>
    /// <response code="400">Bład podczas pobierania</response>
    [HttpGet("DownloadPdf/{ticketID}")]
    public async Task<IActionResult> DownloadPdf(int ticketID)
    {
        try
        {
            string redisKey = $"TicketData:{ticketID}";
            byte[] pdfBytes = await _redisService.GetAsync(redisKey);

            if (pdfBytes != null)
            {
                return File(pdfBytes, "application/pdf", $"ticket-{ticketID}.pdf");
            }
            else
            {
                return NotFound("PDF not found. It may still be generating.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
    
    [ApiExplorerSettings(IgnoreApi = true)]
    public string GetUserIdFromToken()
    {
        var user = HttpContext.User;

        if (user != null && user.Identity.IsAuthenticated)
        {
            var userIdClaim = user.FindFirst("user_id");

            if (userIdClaim != null)
            {
                return userIdClaim.Value;
            }
        }

        return null; // Lub inna wartość oznaczająca brak dostępu do ID użytkownika
    }
    
}
