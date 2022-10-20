using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;

public class APIHelper : MonoBehaviour
{
    public static APIHelper instance;
    private string URL = "http://localhost/api";
    void Awake()
    {
        URL = ENVConfig.api_endpoint;
        instance = this;
    }

    // USER ROUTE
    [System.Serializable]
    public class Token
    {
        public string access_token;
        public User user;
    }
    [System.Serializable]
    public class User
    {
        public int id;
        public string name;
        public string email;
        public string role;
    }
    public IEnumerator login(string username, string password, Action successAction, Action failAction)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("email", username);
        formData.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(URL + "/auth/login", formData);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            failAction();
        }
        else
        {
            Token response = JsonUtility.FromJson<Token>(www.downloadHandler.text);
            LocalUserData.id = response.user.id;
            LocalUserData.auth_token = response.access_token;
            LocalUserData.name = response.user.name;
            LocalUserData.email = response.user.email;
            LocalUserData.role = response.user.role;
            successAction();
        }
    }

    // CLASS EXERCISE ROUTE
    [System.Serializable]
    public class RootClassObject
    {
        public ClassRoom[] classRooms;
    }
    public IEnumerator getClassExercises(Action successAction, Action failAction)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL + "/userClassExercises/" + LocalUserData.id);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            failAction();
        }
        else
        {
            RootClassObject response = JsonUtility.FromJson<RootClassObject>("{\"classRooms\":" + www.downloadHandler.text + "}");
            LocalClassRoomList.classRooms = response.classRooms;
            successAction();
        }
    }


    //QUESTIONs ROUTE
    [System.Serializable]
    public class RootQuestObject
    {
        public Question[] questions;
    }
    public IEnumerator getQuestionAnswer(int exercisID, Action successAction, Action failAction)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL + "/exerciseQuestions/" + exercisID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            failAction();
        }
        else
        {
            RootQuestObject response = JsonUtility.FromJson<RootQuestObject>("{\"questions\":" + www.downloadHandler.text + "}");
            LocalQuestionList.questions = response.questions;
            successAction();
        }
    }



    // CREATE HISTORY
    [System.Serializable]
    public class HistoryObject
    {
        public int class_id;
        public int exercises_id;
        public int id;
    }
    public IEnumerator createHistory(int classID, int exercisID, Action successAction, Action failAction)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("class_id", classID);
        formData.AddField("exercises_id", exercisID);

        UnityWebRequest www = UnityWebRequest.Post(URL + "/history", formData);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            failAction();
        }
        else
        {
            HistoryObject response = JsonUtility.FromJson<HistoryObject>(www.downloadHandler.text);
            PhotonNetwork.CurrentRoom.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable() { { RoomProperty.GameID, response.id } }
            );
            successAction();
        }
    }

    // POST ROUND DATA
    public IEnumerator sendRoundData(int history_id, int question_id, int score, int score_ed,
                                    int fom, int success_time_second,
                                    Action successAction, Action failAction)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("history_id", history_id);
        formData.AddField("question_id", question_id);
        formData.AddField("user_id", LocalUserData.id);
        formData.AddField("score", score);
        formData.AddField("score_ed", score_ed);
        formData.AddField("fom", fom);
        formData.AddField("success_time", success_time_second);

        Debug.Log(
            "history_id: " + history_id + "\n" +
            "question_id: " + question_id + "\n" +
            "user_id: " + LocalUserData.id + "\n" +
            "score: " + score + "\n" +
            "score_ed: " + score_ed + "\n" +
            "fom: " + fom + "\n" +
            "success_time: " + success_time_second + "\n"
        );

        UnityWebRequest www = UnityWebRequest.Post(URL + "/round", formData);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("fail: " + www.error);
            failAction();
        }
        else
        {
            Debug.Log("success: " + www.downloadHandler.text);
            successAction();
        }
    }
}
