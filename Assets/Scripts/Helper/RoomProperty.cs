using System.Collections;
using UnityEngine;

// This Class for Photon Custom Room Properties are named in strings.
// By defining them as constants here, 
// I'm making sure that I won't have any spelling error when using them

public class RoomProperty
{
    public const string Mode = "Mode";
    public const string MaxPlayers = "MaxPlayers";
    public const string PlayerInTeam = "PlayerInTeam";
    public const string Pvp = "Pvp";
    public const string Timelimit = "Timelimit";
    public const string ClassID = "ClassID";
    public const string ExerciseID = "ExerciseID";
    public const string QuestionIndex = "QuestionIndex"; // start at 0
    public const string GameID = "GameID"; // history id
}
