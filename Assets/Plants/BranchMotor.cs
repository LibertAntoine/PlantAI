using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchMotor : MonoBehaviour
{
    private Dictionary<Vector3, float> lightExposition;
    private BranchGrowMotor branchGrowMotor;

    void Start()
    {
        if (!gameObject.GetComponent<BranchMotor>())
            Debug.LogError("GameObject '" + name + "' should have BranchGrowMotor script to manage grow expansion.");
        else
            branchGrowMotor = gameObject.GetComponent<BranchGrowMotor>();
    }

    void Update()
    {
        if (branchGrowMotor)
            branchGrowMotor.Grow(0.0001f);
    }

    ///// Setters /////
    public void SetLightExposition(Dictionary<Vector3, float> exposition)
    {
        lightExposition = exposition;
    }
}
