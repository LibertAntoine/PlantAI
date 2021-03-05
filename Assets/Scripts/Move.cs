using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    bool contains;

    int speed = 13;
    float lastRotation = 0;

    Vector3 newPos;

    // Start is called before the first frame update
    void Start()
    {
        newPos = transform.position;
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;

        if (Input.GetKey(KeyCode.S))
        {
            newPos += transform.TransformDirection(Vector3.left) * Time.deltaTime * speed;         
        }
        if (Input.GetKey(KeyCode.Z))
        {
            newPos += transform.TransformDirection(Vector3.right) * Time.deltaTime * speed;           
        }
        if (Input.GetKey(KeyCode.Q))
        {
            newPos +=  transform.TransformDirection(Vector3.forward) * Time.deltaTime * speed;     
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPos += transform.TransformDirection(Vector3.back) * Time.deltaTime * speed;
        }


        contains = false;
        foreach (GameObject collidObject in GameObject.FindGameObjectsWithTag("ObstacleCamera"))
        {
            if (collidObject.GetComponent<Collider>().bounds.Contains(newPos))
            {
                contains = true;
            }
        }

        if (!contains)
        {
            transform.position =newPos;
        }
        lastRotation = transform.eulerAngles.y;

    }
}
