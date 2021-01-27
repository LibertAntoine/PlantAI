using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafMotor : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }


    // Update is called once per frame
    void Update()
    {
        float factor = 0.1f;
        if (transform.localScale.x < 6)
        {
            transform.localScale += new Vector3(factor, factor, factor);
        }
    }
}
