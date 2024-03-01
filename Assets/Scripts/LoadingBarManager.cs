using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoadingBarManager : MonoBehaviour
{
    private bool _complet = false;
    public bool Compelt
    {
        get { return _complet; }
    }
    Coroutine fadeCoroutine;
    public int ItemID = 0;
    [Range(0, 10)]public float FocusDuration = 3f;
    public Color StartColor = Color.red;
    public Color EndColor = Color.green;
    public UnityEngine.UI.Slider LoadingBar;

    public UnityEngine.UI.Image LoadingBarImage;
    private AudioSource audioSource = null;
    private float WaitSeconds = 0.5f;
    public AudioClip clipCounting;
    public AudioClip clipCorrect;
    Outline outline = null;
    public float SelectionStatus
    {
        get { return LoadingBar.value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadingBar.maxValue = 1;
        LoadingBar.minValue = 0;
        LoadingBar.value = 0;
        LoadingBarImage.color = StartColor;
        LoadingBar.gameObject.SetActive(false);
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        outline = gameObject.GetComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.green;
        outline.OutlineWidth = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeMaterialColor(Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < WaitSeconds)
        {
            yield return null;

            // 更新经过的时间
            elapsedTime += Time.deltaTime;
        }

        LoadingBar.gameObject.SetActive(true);
        LoadingBar.value = 0;
        PlayClip_Counting();
        // 获取相机的位置
        Vector3 cameraPosition = Camera.main.transform.position;

        // 获取Slider的Transform
        Transform sliderTransform = LoadingBar.transform; // 假设OutlineObject有一个公开的Slider属性
        while (elapsedTime < duration)
        {
            // 根据经过的时间计算当前颜色
            Color currentColor = Color.Lerp(startColor, endColor, elapsedTime / duration);

            // 设置材质的颜色
            LoadingBarImage.color = currentColor;
            LoadingBar.value = elapsedTime / duration;


            // 让Slider面向相机
            // 注意：这假设Slider的正面是Z轴正方向。如果不是这样，你可能需要调整它。
            sliderTransform.LookAt(cameraPosition);

            // 如果你不希望Slider倾斜上下，只想它水平旋转来面对相机，可以调整rotation使其只在Y轴旋转
            Vector3 targetDirection = cameraPosition - sliderTransform.position;
            float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            sliderTransform.rotation = Quaternion.Euler(0, targetAngle, 0);


            // 等待下一帧
            yield return null;

            // 更新经过的时间
            elapsedTime += Time.deltaTime;
        }

        // 确保最终颜色是结束颜色
        LoadingBarImage.color = endColor;
        LoadingBar.value = 1;
        _complet = true;
        outline.OutlineWidth = 5f;
        LoadingBar.gameObject.SetActive(false);
        PlayClip_Correct();
    }

    public void Hitted()
    {
        Console.WriteLine("Be hitted");
        //this.GetComponent<Renderer>().materials[1].color = Color.green;
        if (fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(FadeMaterialColor(StartColor, EndColor, FocusDuration + WaitSeconds));
        }
    }
    public void StopHitted()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        LoadingBar.value = 0;
        LoadingBarImage.color = StartColor;
        LoadingBar.gameObject.SetActive(false);
        audioSource.Stop();
    }
    public void PlayClip_Counting()
    {
        audioSource.clip = clipCounting;
        audioSource.Play();
    }

    public void PlayClip_Correct()
    {
        audioSource.clip = clipCorrect;
        audioSource.Play();
    }
}
