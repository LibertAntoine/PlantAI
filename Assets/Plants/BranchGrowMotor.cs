using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchGrowMotor : MonoBehaviour
{
    public float startScale = 0.02f;

    private BranchMotor branchMotor;
    void Awake()
    {

        transform.localScale = new Vector3(startScale, 1, startScale);
        branchMotor = GetComponent<BranchMotor>();
    }

    public void Grow(float scale)
    {
        Vector3 scaleChange = new Vector3(scale, 0, scale);
        transform.localScale += scaleChange; // / (branchMotor.generation + 1);
        if(transform.localScale.x < startScale)
        {
            transform.localScale = new Vector3(startScale, transform.localScale.y, startScale);
        }

        if (branchMotor.parentBranch != null && transform.localScale.x > branchMotor.parentBranch.transform.localScale.x)
        {
            transform.localScale = new Vector3(branchMotor.parentBranch.transform.localScale.x, transform.localScale.y, branchMotor.parentBranch.transform.localScale.x);
        }

    }
}
