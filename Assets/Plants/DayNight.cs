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
    }
}
