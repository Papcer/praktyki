using System.Security.Claims;
using System.Text.RegularExpressions;
using App.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace App.Class;


public class PdfHub : Hub
{
    public async Task NotifyPdfReady(object pdfFileInfo)
    {
        var userId = Context.User?.FindFirst("user_id")?.Value;
        await Clients.Group($"{userId}").SendAsync("NotifyPdfReady", pdfFileInfo);
    }
    
    [Authorize]
    public async Task Join()
    {
        var userId = Context.User?.FindFirst("user_id")?.Value;
        Console.WriteLine(userId);
        Console.WriteLine("dodano do grupy " + Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{userId}");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User?.FindFirst("user_id")?.Value;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{userId}");
        Console.WriteLine("usunieto z grupy" + Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}