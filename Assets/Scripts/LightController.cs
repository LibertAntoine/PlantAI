using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{

    public bool on = true;

    private Light spotLight;
    private GameObject mask;



    void Start()
    {
        spotLight = GetComponentInChildren<Light>();
        mask = transform.Find("Mask").gameObject;
        if(on)
        {
            spotLight.enabled = true;
            mask.SetActive(false);
        }
        else
        {
            spotLight.enabled = false;
            mask.SetActive(true);
        }
    }
    
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            spotLight.enabled = !spotLight.enabled;
            mask.SetActive((spotLight.enabled) ? false : true); 
        }
    }
}
