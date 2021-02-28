using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    bool isNotCollidedLEFT = true;
    bool isNotCollidedRIGHT = true;
    bool isNotCollidedFORWARD = true;
    bool isNotCollidedBACK = true;

    int speed = 13;
    float lastRotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
        if (transform.position.x >= 480)
        {
            //float x = Mathf.Clamp(transform.position.x, 480, 482);
            //transform.position = new Vector3(x, transform.position.y, transform.position.z);
            transform.position = new Vector3(480,transform.position.y,transform.position.z);
        }
        else if (transform.position.x <= 382)
        {
            transform.position = new Vector3(382, transform.position.y, transform.position.z);
        }

        if (transform.position.z >= 732)
        {
            //float x = Mathf.Clamp(transform.position.x, 480, 482);
            //transform.position = new Vector3(x, transform.position.y, transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y, 732);
        }
        else if (transform.position.z <= 640)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 640);
        }

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
        if (Input.GetKey(KeyCode.S)/* && isNotCollidedLEFT*/)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.Z) /*&& isNotCollidedRIGHT*/)
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.Q) /*&& isNotCollidedFORWARD*/)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.D) /*&& isNotCollidedBACK*/)
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);
        }

        lastRotation = transform.eulerAngles.y;
    }

   /* private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with " + collision.gameObject.name);
        if (Input.GetKey(KeyCode.S))
        {
            isNotCollidedLEFT = false;
            //isNotCollidedRIGHT = true;
            //isNotCollidedFORWARD = true;
            //isNotCollidedBACK = true;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            //isNotCollidedLEFT = true;
            isNotCollidedRIGHT = false;
            //isNotCollidedFORWARD = true;
            //isNotCollidedBACK = true;

        }
        if (Input.GetKey(KeyCode.Q))
        {
            //isNotCollidedLEFT = true;
            //isNotCollidedRIGHT = true;
            isNotCollidedFORWARD = false;
            //isNotCollidedBACK = true;

        }
        if (Input.GetKey(KeyCode.D))
        {
            //isNotCollidedLEFT = true;
            //isNotCollidedRIGHT = true;
            //isNotCollidedFORWARD = true;
            isNotCollidedBACK = false;

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        isNotCollidedLEFT = true;
        isNotCollidedRIGHT = true;
        isNotCollidedFORWARD = true;
        isNotCollidedBACK = true;
    }*/

    

}
