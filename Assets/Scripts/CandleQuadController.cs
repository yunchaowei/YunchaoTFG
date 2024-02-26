using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleQuadController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        // 面向相机
        // 注意：这假设Slider的正面是Z轴正方向。如果不是这样，你可能需要调整它。
        this.transform.LookAt(cameraPosition);

        // 如果你不希望Slider倾斜上下，只想它水平旋转来面对相机，可以调整rotation使其只在Y轴旋转
        Vector3 targetDirection = cameraPosition - this.transform.position;
        float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0, targetAngle, 0);
    }
}
