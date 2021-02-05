using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    bool open = false;

    void OnMouseDown()
    {
        print(transform.position.x);
        
            if (!open)
            {
                transform.position = new Vector3(transform.position.x+10, transform.position.y, transform.position.z);
                open = true;
            }
            else
            {
                transform.position = new Vector3(transform.position.x-10, transform.position.y, transform.position.z);
                open = false;
            }

        
        
    }
}