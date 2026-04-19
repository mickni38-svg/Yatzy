namespace Yatzy.Api.Hubs;

public static class HubEvents
{
    public const string GameStateUpdated  = "GameStateUpdated";
    public const string PlayerJoined      = "PlayerJoined";
    public const string PlayerLeft        = "PlayerLeft";
    public const string GameStarted       = "GameStarted";
    public const string DiceRolled        = "DiceRolled";
    public const string HoldChanged       = "HoldChanged";
    public const string ScoreSelected     = "ScoreSelected";
    public const string GameEnded         = "GameEnded";
    public const string Error             = "Error";
}
