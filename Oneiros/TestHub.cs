using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace Oneiros;

public class TestHub : Hub
{
    Dictionary<string, string> LoginTokenMap = new();

    public override async Task<Task> OnConnectedAsync()
    {
        Console.WriteLine($"Client Connected");
        await SendCharacterStatsData();
        return base.OnConnectedAsync();
    }

    public async Task GetLoginToken(string username, string password)
    {
        // Test credentials
        Dictionary<string, string> CredentialMap = new()
        {
            { "testplayer", "playerpw" },
            { "testmod", "modpw" },
            { "testadmin", "adminpw" }
        };


        if (CredentialMap.ContainsKey(username) && CredentialMap[username] == password)
        {
            if (!LoginTokenMap.ContainsKey(username))
            {
                LoginTokenMap[username] = new Guid().ToString();
            }
            await Clients.Caller.SendAsync("ReceiveLoginToken", LoginTokenMap[username]);
        }
    }

    public async Task Login(string loginToken)
    {
        if (String.IsNullOrWhiteSpace(LoginTokenMap.FirstOrDefault(x => x.Value == loginToken).Key))
        {
            await Clients.Caller.SendAsync("LoginResult", false);
            return;
        }

        await Clients.Caller.SendAsync("LoginResult", true);
    }

    public async Task Logout()
    {
        // TODO: implement once there's a backing DB
    }

    public async Task SendCharacterStatsData()
    {
        var dummyStats = new CharacterStatusData
        {
            Guid = "asdf-asdf-1234-5678",
            Name = "Anwain Amberrose",
            Hp = 20
        };
        var serialStats = JsonSerializer.Serialize(dummyStats);
        await Clients.Caller.SendAsync("ReceivePlayerStats", serialStats);
        Console.WriteLine("Sent pstats");
        Console.WriteLine(serialStats);
    }

    public async Task Confirmed()
    {
        Console.WriteLine("Client sent confirmation");
    }
}