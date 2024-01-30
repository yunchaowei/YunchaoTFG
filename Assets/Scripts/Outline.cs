using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Outline : MonoBehaviour
{
    private bool _complet = false;
    public bool Compelt
    {
        get { return _complet; }
    }
    Coroutine fadeCoroutine;
    public float FocusDuration = 3f;
    public Color StartColor = Color.red;
    public Color EndColor = Color.green;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().materials[1].color = StartColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeMaterialColor(Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            // 根据经过的时间计算当前颜色
            Color currentColor = Color.Lerp(startColor, endColor, elapsedTime / duration);

            // 设置材质的颜色
            GetComponent<Renderer>().materials[1].color = currentColor;

            // 等待下一帧
            yield return null;

            // 更新经过的时间
            elapsedTime += Time.deltaTime;
        }

        // 确保最终颜色是结束颜色
        GetComponent<Renderer>().materials[1].color = endColor;
        _complet = true;
    }

    public void Hitted()
    {
        Console.WriteLine("Be hitted");
        //this.GetComponent<Renderer>().materials[1].color = Color.green;
        if (fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(FadeMaterialColor(StartColor, EndColor, FocusDuration));
        }
    }
    public void StopHitted()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        this.GetComponent<Renderer>().materials[1].color = StartColor;
    }
}
