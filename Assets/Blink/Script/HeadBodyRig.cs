using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap_C
{
    public Transform VRTarget;
    public Transform rigTarget;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    public void Map()
    {
        rigTarget.position = VRTarget.TransformPoint(positionOffset);
        rigTarget.rotation = VRTarget.rotation * Quaternion.Euler(rotationOffset);
    }
}

public class HeadBodyRig : MonoBehaviour
{
    public VRMap_C head;
    public VRMap_C rightHand;
    public VRMap_C leftHand;

    public Transform headConstraint;
    Vector3 offset;

    public float turnFactor = 1f;
    public ForwardAxis forwardAxis;

    public enum ForwardAxis
    {
        blue,
        green,
        red
    }

    void Start()
    {
        offset = transform.position - headConstraint.position;
    }

    void FixedUpdate()
    {
        //transform.position = headConstraint.position + offset;
        transform.position = new Vector3(headConstraint.position.x + offset.x, 0, headConstraint.position.z + offset.z);
        Vector3 projectionVector = headConstraint.up;
        switch (forwardAxis)
        {
            case ForwardAxis.green:
                projectionVector = headConstraint.up;
                break;
            case ForwardAxis.blue:
                projectionVector = headConstraint.forward;
                break;
            case ForwardAxis.red:
                projectionVector = headConstraint.right;
                break;
        }
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(projectionVector, Vector3.up).normalized, Time.deltaTime * turnFactor);

        head.Map();
        rightHand.Map();
        leftHand.Map();
    }
}