using System.Collections.Generic;
using UnityEngine;

public static class LocalClassRoomList
{
    public static ClassRoom[] classRooms;

    public static Dictionary<string, int> getClassroomDic()
    {
        Dictionary<string, int> classroomDic = new Dictionary<string, int>();
        foreach (var classroom in classRooms)
        {
            classroomDic.Add(classroom.name, classroom.id);
        }
        return classroomDic;
    }

    public static ClassRoom findClassRoom(string name)
    {
        foreach (var classroom in classRooms)
        {
            if (classroom.name == name)
            {
                return classroom;
            }
        }
        return null;
    }
}
