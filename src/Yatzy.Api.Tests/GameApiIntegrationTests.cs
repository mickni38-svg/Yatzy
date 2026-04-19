using System.Net;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yatzy.Application.DTOs;

namespace Yatzy.Api.Tests;

[TestClass]
public sealed class GameApiIntegrationTests
{
    private static TestWebAppFactory _factory = null!;
    private static HttpClient _client = null!;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        _factory = new TestWebAppFactory();
        _client = _factory.CreateClient();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    // ── Create Game ──────────────────────────────────────────────

    [TestMethod]
    public async Task CreateGame_ValidRequest_Returns201WithRoomCode()
    {
        var request = new CreateGameRequest("Alice");

        var response = await _client.PostAsJsonAsync("/api/games", request);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GameStateResponse>();
        Assert.IsNotNull(body);
        Assert.AreNotEqual(Guid.Empty, body.GameId);
        Assert.IsFalse(string.IsNullOrWhiteSpace(body.RoomCode));
        Assert.AreEqual("WaitingForPlayers", body.Status);
        Assert.AreEqual(1, body.Players.Count);
        Assert.AreEqual("Alice", body.Players[0].DisplayName);
    }

    [TestMethod]
    public async Task CreateGame_EmptyName_Returns400()
    {
        var request = new CreateGameRequest("   ");

        var response = await _client.PostAsJsonAsync("/api/games", request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── Join Game ────────────────────────────────────────────────

    [TestMethod]
    public async Task JoinGame_ExistingRoom_Returns200WithBothPlayers()
    {
        var created = await CreateGameAndGetState("Host");
        var joinRequest = new JoinGameRequest("Guest");

        var response = await _client.PostAsJsonAsync(
            $"/api/games/{created.RoomCode}/join", joinRequest);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GameStateResponse>();
        Assert.IsNotNull(body);
        Assert.AreEqual(2, body.Players.Count);
        Assert.IsTrue(body.Players.Any(p => p.DisplayName == "Guest"));
    }

    [TestMethod]
    public async Task JoinGame_NonExistentRoom_Returns404()
    {
        var joinRequest = new JoinGameRequest("Ghost");

        var response = await _client.PostAsJsonAsync("/api/games/XXXXXX/join", joinRequest);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task JoinGame_EmptyName_Returns400()
    {
        var created = await CreateGameAndGetState("Host");
        var joinRequest = new JoinGameRequest("   ");

        var response = await _client.PostAsJsonAsync(
            $"/api/games/{created.RoomCode}/join", joinRequest);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── Start Game ───────────────────────────────────────────────

    [TestMethod]
    public async Task StartGame_TwoPlayers_Returns200WithStatusInProgress()
    {
        var game = await CreateGameAndGetState("Alice");
        await JoinGameAsGuest(game.RoomCode, "Bob");

        var response = await _client.PostAsync($"/api/games/{game.GameId}/start", null);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GameStateResponse>();
        Assert.IsNotNull(body);
        Assert.AreEqual("InProgress", body.Status);
        Assert.AreEqual(5, body.Dice.Count);
    }

    [TestMethod]
    public async Task StartGame_OnlyOnePlayer_Returns400()
    {
        var game = await CreateGameAndGetState("Solo");

        var response = await _client.PostAsync($"/api/games/{game.GameId}/start", null);

        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [TestMethod]
    public async Task StartGame_NonExistentGame_Returns404()
    {
        var response = await _client.PostAsync($"/api/games/{Guid.NewGuid()}/start", null);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── Host ─────────────────────────────────────────────────────

    [TestMethod]
    public async Task CreateGame_CreatorIsHost()
    {
        var state = await CreateGameAndGetState("Alice");

        Assert.AreEqual(1, state.Players.Count);
        Assert.IsTrue(state.Players[0].IsHost,
            "Den spiller der opretter spillet skal være host.");
        Assert.AreEqual("Alice", state.Players[0].DisplayName);
    }

    [TestMethod]
    public async Task JoinGame_CreatorRemainsHost_AfterSecondPlayerJoins()
    {
        var created = await CreateGameAndGetState("Alice");
        var hostId = created.Players[0].PlayerId;

        var joinResponse = await _client.PostAsJsonAsync(
            $"/api/games/{created.RoomCode}/join", new JoinGameRequest("Bob"));
        joinResponse.EnsureSuccessStatusCode();
        var state = await joinResponse.Content.ReadFromJsonAsync<GameStateResponse>();

        Assert.IsNotNull(state);
        Assert.AreEqual(2, state.Players.Count);

        var host  = state.Players.FirstOrDefault(p => p.IsHost);
        var guest = state.Players.FirstOrDefault(p => !p.IsHost);

        Assert.IsNotNull(host,  "Der skal være præcis én host efter anden spiller joiner.");
        Assert.IsNotNull(guest, "Den anden spiller må ikke være host.");
        Assert.AreEqual(hostId, host.PlayerId,
            "Det skal stadig være den originale opretter der er host.");
        Assert.AreEqual("Alice", host.DisplayName);
        Assert.AreEqual("Bob",   guest.DisplayName);
    }

    [TestMethod]
    public async Task JoinGame_OnlyOnePlayerIsHost()
    {
        var created = await CreateGameAndGetState("Alice");
        await JoinGameAsGuest(created.RoomCode, "Bob");
        await JoinGameAsGuest(created.RoomCode, "Charlie");

        var response = await _client.GetAsync($"/api/games/{created.GameId}");
        response.EnsureSuccessStatusCode();
        var state = await response.Content.ReadFromJsonAsync<GameStateResponse>();

        Assert.IsNotNull(state);
        Assert.AreEqual(3, state.Players.Count);

        var hostCount = state.Players.Count(p => p.IsHost);
        Assert.AreEqual(1, hostCount,
            "Der må kun være præcis én host uanset hvor mange spillere der joiner.");
        Assert.IsTrue(state.Players[0].IsHost,
            "Den første spiller (opretter) skal være host.");
        Assert.IsFalse(state.Players[1].IsHost, "Bob må ikke være host.");
        Assert.IsFalse(state.Players[2].IsHost, "Charlie må ikke være host.");
    }

    // ── Get Game State ───────────────────────────────────────────

    [TestMethod]
    public async Task GetGameState_ExistingGame_Returns200WithCorrectData()
    {
        var created = await CreateGameAndGetState("Alice");

        var response = await _client.GetAsync($"/api/games/{created.GameId}");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GameStateResponse>();
        Assert.IsNotNull(body);
        Assert.AreEqual(created.GameId, body.GameId);
        Assert.AreEqual(created.RoomCode, body.RoomCode);
    }

    [TestMethod]
    public async Task GetGameState_NonExistentGame_Returns404()
    {
        var response = await _client.GetAsync($"/api/games/{Guid.NewGuid()}");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── Helpers ──────────────────────────────────────────────────

    private async Task<GameStateResponse> CreateGameAndGetState(string hostName)
    {
        var response = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest(hostName));
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<GameStateResponse>();
        return body!;
    }

    private async Task JoinGameAsGuest(string roomCode, string guestName)
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/games/{roomCode}/join", new JoinGameRequest(guestName));
        response.EnsureSuccessStatusCode();
    }
}
