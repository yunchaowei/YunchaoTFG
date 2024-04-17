using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SceneManager;

public class SceneManager : MonoBehaviour
{
    public enum HeightOptions
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        VeryHigh = 4
    }
    private int _heightOptions = 0;
    public HeightOptions HeightOption = HeightOptions.Medium;
    public string UserName = "Test User";
    [Range(1.20f, 2.00f)]public float UserHeight;
    [Range(1.00f, 5.00f)] public float MaxDistanceCasting;
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
    public List<GameObject> FurnituresSelectables = new List<GameObject>();
    public GameObject cameraParent;
    public float finalHeight;
    //public GameObject cameraY;
    // Start is called before the first frame update
    void Start()
    {

        //Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, UserHeight, Camera.main.transform.position.z);
       switch (HeightOption)
       {
            case HeightOptions.None:
                _heightOptions = 0;
                break;
            case HeightOptions.Low:
                _heightOptions = 1;
                break;
            case HeightOptions.Medium:
                _heightOptions = 2;
                break;
            case HeightOptions.High:
                _heightOptions = 3;
                break;
            case HeightOptions.VeryHigh:
                _heightOptions = 4;
                break;
        }
        _changeHeights(true);

        StartCoroutine(CollectDataRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Reset Furnitures
            _reset_Furnitures();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _heightOptions++;
            if (_heightOptions > 4)
                _heightOptions = 4;
            else
                _changeHeights(true);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _heightOptions--;
            if (_heightOptions < 0)
                _heightOptions = 0;
            else
                _changeHeights(false);
        }
    }

    private void _changeHeights(bool sum)
    {
        switch (_heightOptions)
        {
            case 0:
                HeightOption = HeightOptions.None;
                if(sum)
                    height = 0;
                else
                    height = -0.05f;
                break;
            case 1:
                HeightOption = HeightOptions.Low;
                if(sum)
                    height = 0.05f;
                else
                    height = -0.1f;
                break;
            case 2:
                HeightOption = HeightOptions.Medium;
                if (sum)
                    height = 0.1f;
                else
                    height = -0.2f;
                break;
            case 3:
                HeightOption = HeightOptions.High;
                if (sum)
                    height = 0.2f;
                else
                    height = -0.5f;
                break;
            case 4:
                HeightOption = HeightOptions.VeryHigh;
                if (sum)
                    height = 0.5f;

                break;
        }

        finalHeight = cameraParent.transform.position.y + height;
        if (CanChangeHeight)
        {
            Furnitures.transform.position = new Vector3(Furnitures.transform.position.x,
                                                        Furnitures.transform.position.y + height,
                                                        Furnitures.transform.position.z);
            foreach (GameObject go in FurnituresScaled)
            {
                go.transform.localScale = new Vector3(go.transform.localScale.x,
                                                      go.transform.localScale.y+height,  //(go.transform.parent.transform.position.y + height)/ go.transform.parent.transform.position.y,
                                                        go.transform.localScale.z);
            }
        }

    }

    private void _reset_Furnitures()
    {
        foreach (GameObject go in FurnituresSelectables)
        {
            LoadingBarManager lbm = go.GetComponent<LoadingBarManager>();
            if (lbm != null)
                lbm.ResetOutLine();
        }
    }

    IEnumerator CollectDataRoutine()
    {
        while (true)
        {
            GameData data = new GameData
            {
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                //playerPosition = Camera.main.transform.position,
                //cameraRotation = Camera.main.transform.rotation,
                playerPosition = cameraParent.transform.position,
                cameraRotation = cameraParent.transform.rotation,
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
    void LateUpdate()
    {

            cameraParent.transform.position = new Vector3(cameraParent.transform.position.x, finalHeight, cameraParent.transform.position.z);
       
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
