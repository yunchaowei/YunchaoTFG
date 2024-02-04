using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCaster : MonoBehaviour
{
    private LoadingBarManager OutlineObject = null;
    GameObject hitObject_Act = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        float maxDistance = 10f;
        RaycastHit hit;
        bool isHit = Physics.BoxCast(transform.position, transform.lossyScale / 2, transform.forward, out hit, transform.rotation, maxDistance);
        if (isHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, transform.lossyScale);

            GameObject hitObject = hit.collider.gameObject;

            if (hitObject != null)
            {
                if (hitObject_Act == null)
                    hitObject_Act = hitObject;
                
                if(hitObject_Act == hitObject)
                {
                    OutlineObject = hitObject.GetComponent<LoadingBarManager>();

                    if (OutlineObject != null)
                    {
                        OutlineObject.Hitted();
                    }
                }
                else
                {
                    hitObject_Act = hitObject;
                    if (OutlineObject != null && !OutlineObject.Compelt)
                    {
                        OutlineObject.StopHitted();
                    }
                }
            }
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * maxDistance);

            if (OutlineObject != null && !OutlineObject.Compelt)
            {
                OutlineObject.StopHitted();
            }
        }
    }
}
