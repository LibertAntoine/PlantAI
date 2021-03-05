using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    public GameObject sun;
    public float speed = 1f;


    // Update is called once per frame
    void Update()
    {
        sun.transform.Rotate(Vector3.right * speed * Time.deltaTime);

        if (Vector3.Dot(sun.transform.forward, Vector3.up) > 0)
        {
            if (sun.gameObject.activeSelf == true)
            {
                sun.gameObject.SetActive(false);
            }

        }
        else
        {
            if (sun.gameObject.activeSelf == false)
            {
                sun.gameObject.SetActive(true);
            }
        }
    }

}
