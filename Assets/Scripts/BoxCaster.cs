using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoxCaster : MonoBehaviour
{
    private LoadingBarManager OutlineObject = null;
    GameObject hitObject_Act = null;
    public GameObject SceneManager = null;
    private SceneManager _sceneManager = null;
    public GameObject _sphereHit;
    private bool isHit = false;
    RaycastHit hit;
    float maxDistance;
    // Start is called before the first frame update
    void Start()
    {
        _sceneManager = SceneManager.GetComponent<SceneManager>();
        //_sphereHit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        maxDistance = _sceneManager != null ? _sceneManager.MaxDistanceCasting : 2.5f;
    }

    private void Update()
    {
        // bool isHit = Physics.BoxCast(transform.position, transform.lossyScale / 3, transform.forward, out hit, transform.rotation, maxDistance);
        isHit = Physics.Raycast(transform.position, transform.forward, out hit, maxDistance);
        if (isHit)
        {
            _sphereHit.transform.position = hit.point;
            _sphereHit.SetActive(true);
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject != null)
            {
                if (hitObject_Act == null)
                    hitObject_Act = hitObject;

                if (hitObject_Act == hitObject)
                {
                    OutlineObject = hitObject.GetComponent<LoadingBarManager>();

                    if (OutlineObject != null)
                    {
                        OutlineObject.Hitted();
                        _sceneManager.lookAtItemID = OutlineObject.ItemID;
                        _sceneManager.lookAtItemName = hitObject_Act.name;
                        _sceneManager.lookAtItemHegiht = hitObject.transform.position.y;
                        _sceneManager.selectionState = OutlineObject.SelectionStatus;
                    }
                }
                else
                {
                    hitObject_Act = hitObject;
                    if (OutlineObject != null && !OutlineObject.Compelt)
                    {
                        OutlineObject.StopHitted();
                        _sceneManager.Clear_ItemState();
                    }
                }
            }
        }
        else
        {
            _sphereHit.SetActive(false);
            if (OutlineObject != null && !OutlineObject.Compelt)
            {
                OutlineObject.StopHitted();
                _sceneManager.Clear_ItemState();
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (isHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        }
    }
}
