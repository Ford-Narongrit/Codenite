using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateRoomController : MonoBehaviourPunCallbacks
{
    [Header("Room Option")]
    [SerializeField] private int RoomCodeLength = 6;
    [SerializeField] private int maxPlayer = 20;
    [SerializeField] private int spectator = 5;
    private RoomOptions roomOptions = new RoomOptions();

    [Header("option")]
    [SerializeField] private OptionField mode;
    [SerializeField] private OptionField pvp;
    [SerializeField] private OptionField timelimit;
    [SerializeField] private OptionField classes;
    [SerializeField] private OptionField exercises;
    public void Start()
    {
        StartCoroutine(APIHelper.instance.getClassExercises(
        () =>
        {
            if (LocalClassRoomList.classRooms.Length <= 0)
            {
                AlertController.Instance.showAlert("Can't create Room",
                "Classroom or Exercise not found, please create classroom or exercise then try again.", "Back",
                () =>
                {
                    SceneManager.LoadScene(SceneConfig.SelectGame);
                });
                return;
            }
            classes.setOptions(LocalClassRoomList.getClassroomDic());
            classes.dropdown.onValueChanged.AddListener(delegate
            {
                onChangeClass();
            });

            if (LocalClassRoomList.classRooms[0].getExerciseNameDic().Count <= 0)
            {
                AlertController.Instance.showAlert("Can't create Room",
                "Classroom or Exercise not found, please create classroom or exercise then try again.", "Back",
                () =>
                {
                    SceneManager.LoadScene(SceneConfig.SelectGame);
                });
                return;
            }
            exercises.setOptions(LocalClassRoomList.classRooms[0].getExerciseNameDic());
        },
        () =>
        {
            AlertController.Instance.showAlert("Connection fail",
            "Please try again.", "Back",
            () =>
            {
                SceneManager.LoadScene(SceneConfig.SelectGame);
            });
            Debug.Log("fail");
        }
        ));
    }
    public void onChangeClass()
    {
        exercises.dropdown.value = 0;
        exercises.setOptions(LocalClassRoomList.
            findClassRoom(classes.currentValue.Key).getExerciseNameDic());
    }
    public void OnClickLeave()
    {
        SceneManager.LoadScene(SceneConfig.SelectGame);
    }
    public void onCilckCreate()
    {
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene(SceneConfig.WaitingRoom);
    }
    private void CreateRoom()
    {
        roomOptions.MaxPlayers = (byte)(maxPlayer + spectator);

        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add(RoomProperty.Mode, mode.currentValue.Key);
        roomOptions.CustomRoomProperties.Add(RoomProperty.MaxPlayers, maxPlayer);
        roomOptions.CustomRoomProperties.Add(RoomProperty.PlayerInTeam, mode.currentValue.Value);
        roomOptions.CustomRoomProperties.Add(RoomProperty.Pvp, pvp.currentValue.Value);
        roomOptions.CustomRoomProperties.Add(RoomProperty.Timelimit, timelimit.currentValue.Value);
        roomOptions.CustomRoomProperties.Add(RoomProperty.ClassID, classes.currentValue.Value);
        roomOptions.CustomRoomProperties.Add(RoomProperty.ExerciseID, exercises.currentValue.Value);
        roomOptions.CustomRoomProperties.Add(RoomProperty.QuestionIndex, 0);
        roomOptions.CustomRoomProperties.Add(RoomProperty.GameID, 0);

        string randomRoomCode = RandomRoomCode(RoomCodeLength);
        PhotonNetwork.CreateRoom(randomRoomCode, roomOptions);
    }
    private string RandomRoomCode(int range)
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string roomCode = "";

        for (int i = 0; i < range; i++)
        {
            roomCode += characters[Random.Range(0, characters.Length)];
        }
        return roomCode;
    }
}
