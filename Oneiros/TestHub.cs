using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using BC = BCrypt.Net.BCrypt;

namespace Oneiros;

public class TestHub : Hub
{
    // Test credentials
    Dictionary<string, string> CredentialMap = new()
    {
        { "testplayer", BC.HashPassword("playerpw") },
        { "testmod", BC.HashPassword("modpw") },
        { "testadmin", BC.HashPassword("adminpw") },
    };
    
    // test login tokens
    Dictionary<string, string> LoginTokenMap = new()
    {
        {"testplayer","tokenuser"},
        {"testmod",""},
        {"testadmin","tokenadmin"}
    };

    public override async Task<Task> OnConnectedAsync()
    {
        Console.WriteLine($"Client Connected");
        return base.OnConnectedAsync();
    }

    public async Task GetLoginToken(string username, string password)
    {
        if (CredentialMap.ContainsKey(username) && BC.Verify(password, CredentialMap[username]))
        {
            if (!LoginTokenMap.ContainsKey(username) || string.IsNullOrWhiteSpace(LoginTokenMap[username]))
            {
                LoginTokenMap[username] = Guid.NewGuid().ToString();
            }
            Console.WriteLine($"Sent login token for {username}: {LoginTokenMap[username]}");
            await Clients.Caller.SendAsync("Receive", "ReceiveLoginToken", LoginTokenMap[username]);
            return;
        }

        Console.WriteLine($"Invalid credentials for {username}");
        await Clients.Caller.SendAsync("Receive", "ReceiveLoginToken", "invalid_credentials");
    }

    public async Task LoginWithToken(string username, string loginToken)
    {
        Random r = new Random();
        if (LoginTokenMap[username] != loginToken)
        {
            await Clients.Caller.SendAsync("Receive", "ReceiveSessionToken", "notoken");
            Console.WriteLine($"Token Login attempted, but the token was not mapped to a username.");
	    return;
        }
        string sessionToken = Guid.NewGuid().ToString();
	await Clients.Caller.SendAsync("Receive", "ReceiveSessionToken", sessionToken);
	Console.WriteLine($"Login token accepted - send session token: {sessionToken}");
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
        await Clients.Caller.SendAsync("Receive", "ReceivePlayerStats", serialStats);
        Console.WriteLine("Sent pstats");
        Console.WriteLine(serialStats);
    }

    public async Task Confirmed()
    {
        Console.WriteLine("Client sent confirmation");
    }
}
