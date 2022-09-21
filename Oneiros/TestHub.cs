using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using BC = BCrypt.Net.BCrypt;

namespace Oneiros;

public class TestHub : Hub
{
    Dictionary<string, string> LoginTokenMap = new()
    {
        {"testuser","tokenuser"},
        {"moduser","tokenmod"},
        {"adminuser","tokenadmin"}
    };

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
            { "testplayer", BC.HashPassword("playerpw") },
            { "testmod", BC.HashPassword("modpw") },
            { "testadmin", BC.HashPassword("adminpw") },
        };


        if (CredentialMap.ContainsKey(username) && BC.Verify(password, CredentialMap[username]))
        {
            if (!LoginTokenMap.ContainsKey(username))
            {
                LoginTokenMap[username] = new Guid().ToString();
            }
            Console.WriteLine($"Sent login token for {username}");
            await Clients.Caller.SendAsync("ReceiveLoginToken", LoginTokenMap[username]);
            return;
        }

        Console.WriteLine($"Invalid credentials for {username}");
        await Clients.Caller.SendAsync("ReceiveLoginToken", "invalid_credentials");
    }

    public async Task LoginWithToken(string loginToken)
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