using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static SceneManager;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.Rendering.DebugUI;

public class SceneManager : MonoBehaviour
{//https://www.youtube.com/watch?v=xk-LLY7scvM
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
    const float MinUserHeight = 0f;
    const float MaxUserHeight = 2f;
    [Range(MinUserHeight, MaxUserHeight)]public float UserHeight;
    [Range(1.00f, 5.00f)] public float MaxDistanceCasting;
    public GameObject Furnitures;
    private float FurnituresOriginalHeight = 0;
    private float OVRCameraRigOriginalHeight = 0;
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
    public GameObject OVRCameraRig;
    public float finalHeight;
    public Scrollbar scrollbarUserHeight;  
    public TextMeshProUGUI texUsertHeight;
    public UnityEngine.UI.Button btnUserConfigSave;
    public TMP_InputField InputFieldUserID;
    public ToggleGroup toggleGroup;
    public GameObject CanvasUserConfig;
    //public GameObject cameraY;
    // Start is called before the first frame update
    void Start()
    {
        if (OVRCameraRig != null)
        {
            //OVRCameraRig.transform.position = new Vector3(OVRCameraRig.transform.position.x,
            //                                            UserHeight - 0.1f,
            //                                            OVRCameraRig.transform.position.z);
        }
        FurnituresOriginalHeight = Furnitures.transform.position.y;
        OVRCameraRigOriginalHeight = OVRCameraRig.transform.position.y;
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
        //_changeHeights(true);
        _setHeights();

        scrollbarUserHeight.onValueChanged.AddListener(UpdateTextUsertHeight);
        btnUserConfigSave.onClick.AddListener(btnUserConfigSaveClicked);
        UpdateTextUsertHeight(scrollbarUserHeight.value);
        StartCoroutine(CollectDataRoutine());
    }

    public void btnUserConfigSaveClicked()
    {
        UserName = InputFieldUserID.text;
        UserHeight = float.Parse(texUsertHeight.text);
        UnityEngine.UI.Toggle selectedToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if(selectedToggle != null)
        {
            switch (selectedToggle.name)
            {
                case "Toggle_None":
                    HeightOption = HeightOptions.None;
                    break;
                case "Toggle_Low":
                    HeightOption = HeightOptions.Low;
                    break;
                case "Toggle_Medium":
                    HeightOption = HeightOptions.Medium;
                    break;
                case "Toggle_High":
                    HeightOption = HeightOptions.High;
                    break;
                case "Toggle_VeryHigh":
                    HeightOption = HeightOptions.VeryHigh;
                    break;
            }
            _setHeights();
        }

        CanvasUserConfig.SetActive(false);
    }

    private void UpdateTextUsertHeight(float arg0)
    {
        float currentHeight = Mathf.Lerp(MinUserHeight, MaxUserHeight, scrollbarUserHeight.value);
        texUsertHeight.text = currentHeight.ToString("0.00")/* + " m"*/;
    }

    void OnDestroy()
    {
        scrollbarUserHeight.onValueChanged.RemoveListener(UpdateTextUsertHeight);
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

        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            Debug.Log("B button was pressed on Right Touch Controller.");
            OpsUserConfigMenu(false);
        }
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Debug.Log("A button was pressed on Right Touch Controller.");
            OpsUserConfigMenu(true);
        }
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            _heightOptions++;
            if (_heightOptions > 4)
                _heightOptions = 4;
            else
                _changeHeights(true);
        }
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            _heightOptions--;
            if (_heightOptions < 0)
                _heightOptions = 0;
            else
                _changeHeights(false);
        }
        
    }

    private void OpsUserConfigMenu(bool open)
    {
        CanvasUserConfig.SetActive(open);
    }

    private void _setHeights()
    {
        float additionalHeight = 0.0f;
        switch (HeightOption)
        {
            case HeightOptions.None:
                break;
            case HeightOptions.Low:
                additionalHeight = 0.15f;
                break;
            case HeightOptions.Medium:
                additionalHeight = 0.3f;
                break;
            case HeightOptions.High:
                additionalHeight = 0.45f;
                break;
            case HeightOptions.VeryHigh:
                additionalHeight = 0.6f;
                break;
        }
        finalHeight = UserHeight + additionalHeight;
        if (CanChangeHeight)
        {
            Furnitures.transform.position = new Vector3(Furnitures.transform.position.x,
                                                        FurnituresOriginalHeight + additionalHeight,
                                                        Furnitures.transform.position.z);
            //foreach (GameObject go in FurnituresScaled)
            //{
            //    go.transform.localScale = new Vector3(go.transform.localScale.x,
            //                                          go.transform.localScale.y+height,  //(go.transform.parent.transform.position.y + height)/ go.transform.parent.transform.position.y,
            //                                            go.transform.localScale.z);
            //}
            if (OVRCameraRig != null)
            {
                finalHeight = OVRCameraRigOriginalHeight + additionalHeight;
            }
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
                    height = -0.15f;
                break;
            case 1:
                HeightOption = HeightOptions.Low;
                if(sum)
                    height = 0.15f;
                else
                    height = -0.3f;
                break;
            case 2:
                HeightOption = HeightOptions.Medium;
                if (sum)
                    height = 0.3f;
                else
                    height = -0.45f;
                break;
            case 3:
                HeightOption = HeightOptions.High;
                if (sum)
                    height = 0.45f;
                else
                    height = -0.6f;
                break;
            case 4:
                HeightOption = HeightOptions.VeryHigh;
                if (sum)
                    height = 0.6f;

                break;
        }

        finalHeight = cameraParent.transform.position.y + height;
        if (CanChangeHeight)
        {
            Furnitures.transform.position = new Vector3(Furnitures.transform.position.x,
                                                        Furnitures.transform.position.y + height,
                                                        Furnitures.transform.position.z);
            //foreach (GameObject go in FurnituresScaled)
            //{
            //    go.transform.localScale = new Vector3(go.transform.localScale.x,
            //                                          go.transform.localScale.y+height,  //(go.transform.parent.transform.position.y + height)/ go.transform.parent.transform.position.y,
            //                                            go.transform.localScale.z);
            //}
            if (OVRCameraRig != null)
            {
                finalHeight = OVRCameraRig.transform.position.y + height;
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
        if (OVRCameraRig != null)
        {
            OVRCameraRig.transform.position = new Vector3(OVRCameraRig.transform.position.x,
                                                        finalHeight,
                                                        OVRCameraRig.transform.position.z);
        }
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
        File.WriteAllText(UnityEngine.Application.dataPath + $"/DataFolder/{UserName}.json", json);
        Debug.Log("Data saved in path: " + UnityEngine.Application.dataPath + $"/DataFolder/{UserName}.json");
    }
}
