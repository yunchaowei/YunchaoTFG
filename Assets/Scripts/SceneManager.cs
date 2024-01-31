using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public GameObject Furnitures;
    public bool CanChangeHeight = false;
    public float height = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (CanChangeHeight)
        {
            foreach(GameObject go in Furnitures.GetComponentsInChildren<GameObject>(true))
            {
                Vector3 v = go.GetComponent<Transform>().position;
                v = new Vector3(v.x, v.y+height, v.y);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
