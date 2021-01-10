using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchMotor : MonoBehaviour
{
    private Dictionary<Vector3, float> lightExposition;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    ///// Setters /////
    public void SetLightExposition(Dictionary<Vector3, float> exposition)
    {
        lightExposition = exposition;
    }
}
