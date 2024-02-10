using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public long timestamp;
    public Vector3 playerPosition;
    public Quaternion cameraRotation;
    public int LookAtItemID;
    public string LookAtItemName;
    public float LookAtItemHegiht;
    public float SelectionState;
}

[Serializable]
public class GameDataList
{
    public string userName;
    public float userHeight;
    public List<GameData> data = new List<GameData>();
}
