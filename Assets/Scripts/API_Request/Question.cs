using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public int id;
    public string difficulty;
    public string name;
    public string question;
    public string result;
    public string[] answers;
    public string[] correctAnswer;
}
