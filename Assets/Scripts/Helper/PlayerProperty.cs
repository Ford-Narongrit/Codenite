using UnityEngine;
using ExitGames.Client.Photon;
// This Class for Photon Custom Player Properties are named in strings.
// By defining them as constants here, 
// I'm making sure that I won't have any spelling error when using them

// aka key for playerRoomProerties
public class PlayerProperty
{
    public const string Team = "Team";
    public const string Color = "Color";
    public const string IsReady = "IsReady";
    public const string IsSpectator = "IsSpectator";
    public const string IsEliminate = "IsEliminate";
    public const string IsPass = "IsPass";
    public const string Score = "Score";
    public const string Fom = "Fom";
    public const string SuccessTime = "SuccessTime";

    public static Hashtable getInitPlayerProperty()
    {
        Hashtable player_default = new Hashtable();
        player_default[PlayerProperty.Team] = null;
        player_default[PlayerProperty.Color] = null;
        player_default[PlayerProperty.IsReady] = false;
        player_default[PlayerProperty.IsSpectator] = false;
        player_default[PlayerProperty.IsEliminate] = false;
        player_default[PlayerProperty.IsPass] = false;
        player_default[PlayerProperty.Score] = 0;
        player_default[PlayerProperty.Fom] = 0;
        player_default[PlayerProperty.SuccessTime] = 0;
        return player_default;
    }
}
