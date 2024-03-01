using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public enum HeightOptions
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

    public HeightOptions HeightOption = HeightOptions.Medium;
    public string UserName = "Test User";
    [Range(1.20f, 2.00f)]public float UserHeight;
    public GameObject Furnitures;
    public bool CanChangeHeight = false;
    //[Range(0.01f, 0.50f)] public float height = 0;
    private float height = 0;
    private List<GameData> gameDataList = new List<GameData>();
    public int lookAtItemID;
    public string lookAtItemName;
    public float lookAtItemHegiht;
    public float selectionState;
    public List<GameObject> FurnituresScaled = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        switch (HeightOption)
        {
            case HeightOptions.Low:
                height = 0;
                break;
            case HeightOptions.Medium:
                height = 0.1f;
                break;
            case HeightOptions.High:
                height = 0.2f;
                break;
        }
        if (CanChangeHeight)
        {
            Furnitures.transform.position = new Vector3(Furnitures.transform.position.x,
                                                        Furnitures.transform.position.y + height, 
                                                        Furnitures.transform.position.z);
            foreach(GameObject go in FurnituresScaled)
            {
                go.transform.localScale = new Vector3(go.transform.localScale.x,
                                                        go.transform.localScale.y + height,
                                                        go.transform.localScale.z);
            }
        }
        StartCoroutine(CollectDataRoutine());
    }

    IEnumerator CollectDataRoutine()
    {
        while (true)
        {
            GameData data = new GameData
            {
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                playerPosition = Camera.main.transform.position,
                cameraRotation = Camera.main.transform.rotation,
                LookAtItemID = lookAtItemID,
                LookAtItemName = lookAtItemName,
                LookAtItemHegiht = lookAtItemHegiht,
                SelectionState = selectionState,
            };

            gameDataList.Add(data);

            yield return new WaitForSeconds(0.5f); // Collect data every 0.5 seconds
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear_ItemState()
    {
        lookAtItemID = 0;
        lookAtItemName = string.Empty;
        selectionState = 0f;
        lookAtItemHegiht = 0f;
    }

    void OnApplicationQuit()
    {
        SaveGameData();
    }

    void SaveGameData()
    {
        GameDataList dataList = new GameDataList();
        dataList.data = gameDataList;
        dataList.userName = UserName;
        dataList.userHeight = UserHeight;
        string json = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(Application.dataPath + $"/DataFolder/{UserName}.json", json);
        Debug.Log("Data saved in path: " + Application.dataPath + $"/DataFolder/{UserName}.json");
    }
}
