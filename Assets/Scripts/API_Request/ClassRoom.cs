using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ClassRoom
{
    [System.Serializable]
    public class Exercise
    {
        public int id;
        public string name;
    }

    public int id;
    public string name;
    public Exercise[] exercises;

    public Dictionary<string, int> getExerciseNameDic()
    {
        Dictionary<string, int> exerciseNameDic = new Dictionary<string, int>();
        foreach (var exercise in exercises)
        {
            exerciseNameDic.Add(exercise.name, exercise.id);
        }
        return exerciseNameDic;
    }
}
