using Microsoft.AspNetCore.SignalR;

namespace App.Services;

public interface IUserIdProvider
{
    string GetUserId(HubConnectionContext connection);
}