using Oculus.Platform;
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
    [Range(0.01f, 5.00f)] public float MaxDistanceCasting;
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
    public UnityEngine.UI.Toggle ToggleSelecNone;
    public UnityEngine.UI.Toggle ToggleSelecLow;
    public UnityEngine.UI.Toggle ToggleSelecMedium;
    public UnityEngine.UI.Toggle ToggleSelecHigh;
    public UnityEngine.UI.Toggle ToggleSelecVeryHigh;
    public GameObject CanvasUserConfig;
    public GameObject VRCharacterIK;
    public GameObject VRCharacterIK2;
    public GameObject VRCharacterIK3;
    public GameObject VRCharacterIK4;
    public GameObject VRCharacterIK5;
    public GameObject CenterEyeAnchor;
    public GameObject CenterEyeAnchor_Follwer;

    float userBaseHeight = 1.7f;
    const float defaultCharacterHeight = 1.7f;
    private const string CounterKey = "runCounter";
    //public GameObject cameraY;
    // Start is called before the first frame update
    void Start()
    {
        _incrementRunCounter();
        if (OVRCameraRig != null)
        {
            //OVRCameraRig.transform.position = new Vector3(OVRCameraRig.transform.position.x,
            //                                            UserHeight - 0.1f,
            //                                            OVRCameraRig.transform.position.z);
        }
        _changeVRBodyHeight(userBaseHeight - defaultCharacterHeight);
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

    private void _incrementRunCounter()
    {
        int runCount = PlayerPrefs.GetInt(CounterKey, 0);

        runCount++;

        PlayerPrefs.SetInt(CounterKey, runCount);

        Debug.Log($"This application has been run {runCount} times.");

        PlayerPrefs.Save();
    }

    public void btnUserConfigSaveClicked()
    {
        UserName = InputFieldUserID.text;
        //UserHeight = float.Parse(texUsertHeight.text);
        UserHeight = CenterEyeAnchor.transform.position.y;
        UnityEngine.UI.Toggle selectedToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if(selectedToggle != null)
        {
            float addi_Value = 0f;
            switch (selectedToggle.name)
            {
                case "Toggle_None":
                    addi_Value = additionalValue(HeightOption, HeightOptions.None);
                    HeightOption = HeightOptions.None;
                    break;
                case "Toggle_Low":
                    addi_Value = additionalValue(HeightOption, HeightOptions.Low);
                    HeightOption = HeightOptions.Low;
                    break;
                case "Toggle_Medium":
                    addi_Value = additionalValue(HeightOption, HeightOptions.Medium);
                    HeightOption = HeightOptions.Medium;
                    break;
                case "Toggle_High":
                    addi_Value = additionalValue(HeightOption, HeightOptions.High);
                    HeightOption = HeightOptions.High;
                    break;
                case "Toggle_VeryHigh":
                    addi_Value = additionalValue(HeightOption, HeightOptions.VeryHigh);
                    HeightOption = HeightOptions.VeryHigh;
                    break;
            }
            _setHeights();
        }

        CanvasUserConfig.SetActive(false);
    }

    private float additionalValue(HeightOptions previous, HeightOptions now)
    {
        float res = 0;

        switch (previous)
        {
            case HeightOptions.None:
                switch (now)
                {
                    case HeightOptions.None:
                        res += 0;
                        break;
                    case HeightOptions.Low:
                        res += 0.15f * 1;
                        break;
                    case HeightOptions.Medium:
                        res += 0.15f * 2;
                        break;
                    case HeightOptions.High:
                        res += 0.15f * 3;
                        break;
                    case HeightOptions.VeryHigh:
                        res += 0.15f * 4;
                        break;
                }
                break;
            case HeightOptions.Low:
                switch (now)
                {
                    case HeightOptions.None:
                        res -= 0.15f;
                        break;
                    case HeightOptions.Low:
                        res += 0;
                        break;
                    case HeightOptions.Medium:
                        res += 0.15f * 1;
                        break;
                    case HeightOptions.High:
                        res += 0.15f * 2;
                        break;
                    case HeightOptions.VeryHigh:
                        res += 0.15f * 3;
                        break;
                }
                break;
            case HeightOptions.Medium:
                switch (now)
                {
                    case HeightOptions.None:
                        res -= 0.15f*2;
                        break;
                    case HeightOptions.Low:
                        res -= 0.15f * 1;
                        break;
                    case HeightOptions.Medium:
                        res += 0;
                        break;
                    case HeightOptions.High:
                        res += 0.15f * 1;
                        break;
                    case HeightOptions.VeryHigh:
                        res += 0.15f * 2;
                        break;
                }
                break;
            case HeightOptions.High:
                switch (now)
                {
                    case HeightOptions.None:
                        res -= 0.15f * 3;
                        break;
                    case HeightOptions.Low:
                        res -= 0.15f * 2;
                        break;
                    case HeightOptions.Medium:
                        res -= 0.15f * 1;
                        break;
                    case HeightOptions.High:
                        res += 0;
                        break;
                    case HeightOptions.VeryHigh:
                        res += 0.15f * 1;
                        break;
                }
                break;
            case HeightOptions.VeryHigh:
                switch (now)
                {
                    case HeightOptions.None:
                        res -= 0.15f * 4;
                        break;
                    case HeightOptions.Low:
                        res -= 0.15f * 3;
                        break;
                    case HeightOptions.Medium:
                        res -= 0.15f * 2;
                        break;
                    case HeightOptions.High:
                        res -= 0.15f * 1;
                        break;
                    case HeightOptions.VeryHigh:
                        res += 0;
                        break;
                }
                break;
        }

        return res;
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
            _setHeightEum(_heightOptions);
            if (_heightOptions > 4)
                _heightOptions = 4;
            else
                _changeHeights(true);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _heightOptions--; 
            _setHeightEum(_heightOptions);
            if (_heightOptions < 0)
                _heightOptions = 0;
            else
                _changeHeights(false);
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            _reset_Furnitures();//Right Hand Trigger
        }

        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            userBaseHeight = CenterEyeAnchor.transform.position.y;
            Debug.Log("B button was pressed on Right Touch Controller.");
            OpsUserConfigMenu(false);
        }
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Debug.Log("A button was pressed on Right Touch Controller.");
            OpsUserConfigMenu(true);
        }
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            _heightOptions++;
            if (_heightOptions > 4)
                _heightOptions = 4;
            else
                _changeHeights(true);
        }
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            _heightOptions--;
            if (_heightOptions < 0)
                _heightOptions = 0;
            else
                _changeHeights(false);
        }
        
    }

    private void _setHeightEum(int heightOptions)
    {
        switch (heightOptions)
        {
            case 0:
                HeightOption = HeightOptions.None;
                break;
            case 1:
                HeightOption = HeightOptions.Low;
                break;
            case 2:
                HeightOption = HeightOptions.Medium;
                break;
            case 3:
                HeightOption = HeightOptions.High;
                break;
            case 4:
                HeightOption = HeightOptions.VeryHigh;
                break;
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
            _changeVRBodyHeight(finalHeight);

            //VRCharacterIK.transform.localScale = new Vector3(VRCharacterIK.transform.localScale.x, VRCharacterIK.transform.localScale.y + additionalHeight, VRCharacterIK.transform.localScale.z);
        }
    }

    private void _changeVRBodyHeight(float extraHeight)
    {
        if (VRCharacterIK != null)
        {
            if (Mathf.Abs(userBaseHeight) < Mathf.Epsilon) 
            {
                Debug.LogError("Base height is too close to zero, which is not valid for scaling.");
                return;
            }

            float targetHeight = userBaseHeight + extraHeight;

            float scaleFactor = targetHeight / userBaseHeight;//1.088, 1.1764, 1.2647, 1.3529

            //VRCharacterIK.transform.localScale = new Vector3(VRCharacterIK.transform.localScale.x, scaleFactor, VRCharacterIK.transform.localScale.z);
            //VRCharacterIK.transform.position = new Vector3(VRCharacterIK.transform.position.x, VRCharacterIK.transform.position.y+ extraHeight, VRCharacterIK.transform.position.z);
            switch (HeightOption)
            {
                case HeightOptions.None:
                    toggleGroup.SetAllTogglesOff();
                    ToggleSelecNone.isOn = true;
                    VRCharacterIK.SetActive(true);
                    VRCharacterIK2.SetActive(false);
                    VRCharacterIK3.SetActive(false);
                    VRCharacterIK4.SetActive(false);
                    VRCharacterIK5.SetActive(false);
                    break;
                case HeightOptions.Low:
                    toggleGroup.SetAllTogglesOff();
                    ToggleSelecLow.isOn = true;
                    VRCharacterIK.SetActive(false);
                    VRCharacterIK2.SetActive(true);
                    VRCharacterIK3.SetActive(false);
                    VRCharacterIK4.SetActive(false);
                    VRCharacterIK5.SetActive(false);
                    break;
                case HeightOptions.Medium:
                    toggleGroup.SetAllTogglesOff();
                    ToggleSelecMedium.isOn = true;
                    VRCharacterIK.SetActive(false);
                    VRCharacterIK2.SetActive(false);
                    VRCharacterIK3.SetActive(true);
                    VRCharacterIK4.SetActive(false);
                    VRCharacterIK5.SetActive(false);
                    break;
                case HeightOptions.High:
                    toggleGroup.SetAllTogglesOff();
                    ToggleSelecHigh.isOn = true;
                    VRCharacterIK.SetActive(false);
                    VRCharacterIK2.SetActive(false);
                    VRCharacterIK3.SetActive(false);
                    VRCharacterIK4.SetActive(true);
                    VRCharacterIK5.SetActive(false);
                    break;
                case HeightOptions.VeryHigh:
                    toggleGroup.SetAllTogglesOff();
                    ToggleSelecVeryHigh.isOn = true;
                    VRCharacterIK.SetActive(false);
                    VRCharacterIK2.SetActive(false);
                    VRCharacterIK3.SetActive(false);
                    VRCharacterIK4.SetActive(false);
                    VRCharacterIK5.SetActive(true);
                    break;
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
                    height = -0.15f;
                break;
            case 2:
                HeightOption = HeightOptions.Medium;
                if (sum)
                    height = 0.15f;
                else
                    height = -0.15f;
                break;
            case 3:
                HeightOption = HeightOptions.High;
                if (sum)
                    height = 0.15f;
                else
                    height = -0.15f;
                break;
            case 4:
                HeightOption = HeightOptions.VeryHigh;
                if (sum)
                    height = 0.15f;

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
            _changeVRBodyHeight(finalHeight);

            //VRCharacterIK.transform.localScale = new Vector3(VRCharacterIK.transform.localScale.x, VRCharacterIK.transform.localScale.y + height, VRCharacterIK.transform.localScale.z);

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
                playerPosition = CenterEyeAnchor.transform.position,
                cameraRotation = CenterEyeAnchor.transform.rotation,
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

        // elbowR.transform.localScale = new Vector3(Vector3.Distance(elbowR.transform.position, pivotR.transform.position), elbowR.transform.localScale.y, elbowR.transform.localScale.z);
        //VRCharacterIK.transform.position = new Vector3(VRCharacterIK.transform.position.x, VRCharacterIK.transform.position.y-finalHeight, VRCharacterIK.transform.position.z);
        CenterEyeAnchor_Follwer.transform.position = new Vector3(CenterEyeAnchor.transform.position.x, userBaseHeight,CenterEyeAnchor.transform.position.z + 0.1f);
        CenterEyeAnchor_Follwer.transform.rotation = CenterEyeAnchor.transform.rotation;
        VRCharacterIK.transform.position = new Vector3(VRCharacterIK.transform.position.x, 0, VRCharacterIK.transform.position.z);
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
        int key = PlayerPrefs.GetInt(CounterKey);
        UserName = $"User_{key}";
        GameDataList dataList = new GameDataList();
        dataList.data = gameDataList;
        dataList.userName = UserName;
        dataList.userHeight = UserHeight;
        string json = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(UnityEngine.Application.dataPath + $"/DataFolder/{UserName}.json", json);
        Debug.Log("Data saved in path: " + UnityEngine.Application.dataPath + $"/DataFolder/{UserName}.json");
    }
}
