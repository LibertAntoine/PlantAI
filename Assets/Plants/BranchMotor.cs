using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PlantAI;

public class BranchMotor : MonoBehaviour
{



    private float lightSeuilOfDeath = 15000f;

    private float energieNeedForGrow = 200000f;

    private float energieForNewBranch = 40000000f;
    private float accumulatedEnergie = 0;

    /// <summary>Light information recup from LightDetectionMotor</summary>
    private Dictionary<Vector3, float> lightExposition;

    /// <summary>Regular skeleton points along </summary>
    private Dictionary<Vector3, Vector3> skeletonPoints = new Dictionary<Vector3, Vector3>();

    /// <summary>Branch script dependancies</summary>
    private BranchColorMotor branchColorMotor;
    private BranchAnimator branchAnimator;
    private BranchCreatorMotor branchCreatorMotor;

    private bool haveChilds = false;

    void Awake()
    {
        if (!gameObject.GetComponent<BranchColorMotor>())
            Debug.LogError("GameObject '" + name + "' should have BranchColorMotor script to manage the color of the branch.");
        else
            branchColorMotor = gameObject.GetComponent<BranchColorMotor>();

        if (!gameObject.GetComponent<BranchAnimator>())
            Debug.LogError("GameObject '" + name + "' should have BranchAnimator script to manage grow expansion.");
        else
            branchAnimator = gameObject.GetComponent<BranchAnimator>();

        if (!gameObject.GetComponent<BranchCreatorMotor>())
            Debug.LogError("GameObject '" + name + "' should have BranchCreatorMotor script to manage new branch creation.");
        else
            branchCreatorMotor = gameObject.GetComponent<BranchCreatorMotor>();
    }


    void Update()
    {
        float growFactor = (GetGlobalLightExposition() - lightSeuilOfDeath) / energieNeedForGrow;
        accumulatedEnergie += Mathf.Max(GetGlobalLightExposition() - lightSeuilOfDeath, 0);


        if (branchColorMotor)
            branchColorMotor.UpdateColor();

        if (branchAnimator)
        {
            branchAnimator.UpdateAnimation(growFactor);
            branchAnimator.Grow(0.0004f * growFactor);
        }

        if (accumulatedEnergie > energieForNewBranch && !haveChilds)
        {
            haveChilds = true;
            branchCreatorMotor.CreateNewChildBranch(giveRandomSkeletonPoint());
        }

    }

    public void CreateNewLeaf()
    {
        KeyValuePair<Vector3, Vector3> positionAndNormal = skeletonPoints.ElementAt(skeletonPoints.Count - 1);
        Vector3 RandomInfluenceDirection = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));

        Quaternion quat = Quaternion.FromToRotation(Vector3.up, positionAndNormal.Value) * Quaternion.FromToRotation(positionAndNormal.Value, transform.InverseTransformDirection(giveMostExposedDirection()));
        GameObject leaf = Instantiate((GameObject)Resources.Load("Prefabs/Leave", typeof(GameObject)), positionAndNormal.Key, quat, GameObject.FindGameObjectsWithTag("ParentLeaf")[0].transform);
    }

    public void AddSkeletonPoint(Vector3 position, Vector3 normal)
    {
        normal = transform.rotation * normal;
        skeletonPoints.Add(position, normal);
        CreateNewLeaf();
    }

    private Vector3 giveMostExposedDirection()
    {
        if (lightExposition == null) return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        KeyValuePair<Vector3, float> mostExposedDirection;
        foreach (KeyValuePair<Vector3, float> lightDirection in lightExposition)
        {
            if (mostExposedDirection.Value < lightDirection.Value)
            {
                mostExposedDirection = lightDirection;
            }
        }
        return mostExposedDirection.Key;
    }

    private KeyValuePair<Vector3, Vector3> giveRandomSkeletonPoint()
    {
 

        return skeletonPoints.ElementAt(Random.Range(0, skeletonPoints.Count - 1));
    }

    ///// Setters /////
    public void SetLightExposition(Dictionary<Vector3, float> exposition)
    {
        lightExposition = exposition;
    }

    ///// Getters /////
    public float GetGlobalLightExposition()
    {
        if (lightExposition == null) { return lightSeuilOfDeath; }
        float globalExposition = 0f;
        foreach (KeyValuePair<Vector3, float> lightDirection in lightExposition)
            globalExposition += lightDirection.Value;
        return globalExposition / lightExposition.Count;
    }
}
