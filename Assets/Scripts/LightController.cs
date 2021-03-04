using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{

    public bool on = true;

    private Light light;
    private GameObject mask;



    void Start()
    {
        light = GetComponentInChildren<Light>();
        mask = transform.Find("Mask").gameObject;
        if(on)
        {
            light.enabled = true;
            mask.SetActive(false);
        }
        else
        {
            light.enabled = false;
            mask.SetActive(true);
        }
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
