using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{

    Light light;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
     
            light.enabled = !light.enabled;
        }
    }
}
