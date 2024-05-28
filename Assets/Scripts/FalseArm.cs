using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseArm : MonoBehaviour
{

    public float lengthArm1, lengthArm2;

    public GameObject arm1, arm2;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        arm1.transform.position = transform.position;
        arm1.transform.rotation = this.gameObject.transform.rotation;
        
    }
}
