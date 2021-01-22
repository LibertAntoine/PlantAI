using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchGrowMotor : MonoBehaviour
{
    public float startScale = 0.02f;

    void Start()
    {

        transform.localScale = new Vector3(startScale, 1, startScale);
    }

    public void Grow(float scale)
    {
        Vector3 scaleChange = new Vector3(scale, 0, scale);
        transform.localScale += scaleChange;
    }
}
