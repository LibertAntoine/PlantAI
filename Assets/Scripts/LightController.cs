using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{

    private Light light;
    private GameObject mask;

    void Start()
    {
        light = GetComponentInChildren<Light>();
        mask = transform.Find("Mask").gameObject;
        mask.SetActive(false);
    }
    
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            light.enabled = !light.enabled;
            mask.SetActive((light.enabled) ? false : true); 
        }
    }
}
