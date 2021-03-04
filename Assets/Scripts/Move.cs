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

        if (contains)
        {
            newPos = transform.position;
        }

<<<<<<< Updated upstream
        /*if(transform.eulerAngles.y != lastRotation)
        {
            isNotCollidedLEFT = true;
            isNotCollidedRIGHT = true;
            isNotCollidedFORWARD = true;
            isNotCollidedBACK = true;
        }*/

        //remove the /**/ to allow up and down.
        /*if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.up * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }*/

        Vector3 newPos = transform.position;

        if (Input.GetKey(KeyCode.S)/* && isNotCollidedLEFT*/)
=======
        if (Input.GetKey(KeyCode.S))
>>>>>>> Stashed changes
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
                Debug.Log(newPos);

            }
        }

        if (!contains)
        {
            transform.position =newPos;
        }
        lastRotation = transform.eulerAngles.y;

    }
}
