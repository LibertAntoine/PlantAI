using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityRoof : MonoBehaviour
{
    float increment = 0.1f;
    float opacity;
    Color color;
    // Start is called before the first frame update
    void Start()
    {
        color = this.GetComponent<Renderer>().material.GetColor("_Color");
        opacity = color.a;
    }

    // Update is called once per frame
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && opacity < 1)
        {   if(opacity >= 1 - increment)
            {
                this.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, 1));
            } else
            {
                opacity = this.GetComponent<Renderer>().material.GetColor("_Color").a + increment;
                this.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, opacity));
            }

        }

        else if (Input.GetMouseButtonDown(1) && opacity > 0)
        {
            if (opacity <= increment)
            {
                this.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, 0));
            } else
            {
                opacity = this.GetComponent<Renderer>().material.GetColor("_Color").a - increment;
                this.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, opacity));
            }

        }
    }
}
