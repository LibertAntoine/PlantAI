using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    private bool open = false;
    private MeshRenderer partWall;

    void Start()
    {
        partWall = GetComponent<MeshRenderer>();
    }

    void OnMouseDown()
    {
        
        if (!open)
        {
            partWall.enabled = false;
            //transform.position = new Vector3(transform.position.x+10, transform.position.y, transform.position.z);
            open = true;
        } else
        {
            partWall.enabled = true;
            //transform.position = new Vector3(transform.position.x-10, transform.position.y, transform.position.z);
            open = false;
        }
    }
}