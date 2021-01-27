using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PlantAI;

public class BranchMotor : MonoBehaviour
{


    private float lightSeuilOfDeaph = 15000f;
    private float lightNeedForGrowFactor = 200000f;
    private float newBranchFactor = 50000000f;

    private Dictionary<Vector3, float> lightExposition;
    private Dictionary<Vector3, Vector3> skeletonPoints = new Dictionary<Vector3, Vector3>();

    private BranchColorMotor branchColorMotor;
    private BranchAnimator branchAnimator;

    public GameObject parentBranch;
    public int generation = 0;

    private float accumulateEnergie = 0;
    private bool haveChilds = false;

    void Awake()
    {
        if (!gameObject.GetComponent<BranchColorMotor>())
            Debug.LogError("GameObject '" + name + "' should have BranchColorMotor script to manage grow expansion.");
        else
            branchColorMotor = gameObject.GetComponent<BranchColorMotor>();

        if (!gameObject.GetComponent<BranchAnimator>())
            Debug.LogError("GameObject '" + name + "' should have BranchAnimator script to manage grow expansion.");
        else
            branchAnimator = gameObject.GetComponent<BranchAnimator>();
    }

    void Start()
    {
   
    }

    void Update()
    {
        float growFactor = (GetGlobalLightExposition() - lightSeuilOfDeaph) / lightNeedForGrowFactor;
        accumulateEnergie += Mathf.Max(GetGlobalLightExposition() - lightSeuilOfDeaph, 0);

        //Debug.Log(accumulateEnergie);


        if (branchColorMotor)
            branchColorMotor.UpdateColor();

        if (branchAnimator)
        {
            branchAnimator.UpdateAnimation(growFactor);
            branchAnimator.Grow(0.0005f * growFactor);
        }

        if (accumulateEnergie > newBranchFactor && !haveChilds)
        {
            haveChilds = true;
            CreateNewChildBranch(giveMostExposedDirection(), giveRandomSkeletonPoint());
        }

    }

    public void CreateNewChildBranch(Vector3 direction, KeyValuePair<Vector3, Vector3> positionAndNormal)
    {

        Vector3 RandomInfluenceDirection = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0.2f, 0.8f), Random.Range(-0.1f, 0.1f));

        Quaternion quat = Quaternion.FromToRotation(positionAndNormal.Value, direction + RandomInfluenceDirection);
        GameObject branch = (GameObject)Resources.Load("Prefabs/Branch", typeof(GameObject));
        BranchMotor childBranchMotor = Instantiate(branch, positionAndNormal.Key, quat, transform).GetComponent<BranchMotor>();
        childBranchMotor.parentBranch = gameObject;
        childBranchMotor.generation = generation + 1;


    }

    /// <summary>
    /// Create a branch in the continuity of
    /// the given position and normal.
    /// </summary>
    /// <param name="positionAndNormal">Position (Vector3) and Normal (Vector3).</param>
    /// <param name="radius">Base radius of the branch.</param>
    public void CreateNewChildBranchContinuity(KeyValuePair<Vector3, Vector3> positionAndNormal, float radius)
    {
        Quaternion quat = Quaternion.FromToRotation(Vector3.up, positionAndNormal.Value);
        GameObject branchPrefab = (GameObject)Resources.Load("Prefabs/Branch", typeof(GameObject));
        GameObject branch = Instantiate(branchPrefab, positionAndNormal.Key, quat, transform);
        // Immediately set its radius.
        branch.GetComponent<BranchAnimator>().radius = radius;
    }

    public void CreateNewLeaf()
    {
        KeyValuePair<Vector3, Vector3> positionAndNormal = skeletonPoints.ElementAt(skeletonPoints.Count - 1);
        Vector3 RandomInfluenceDirection = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));

        Quaternion quat = Quaternion.FromToRotation(positionAndNormal.Value, giveMostExposedDirection() + RandomInfluenceDirection);
        GameObject leaf = (GameObject)Resources.Load("Prefabs/Leave_a_02", typeof(GameObject));
        
        Instantiate(leaf, positionAndNormal.Key, quat, GameObject.FindGameObjectsWithTag("ParentLeaf")[0].transform).transform.localScale *= 3;
    }

    public void AddSkeletonPoint(Vector3 position, Vector3 normal)
    {
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
        return skeletonPoints.ElementAt(Random.Range(0, skeletonPoints.Count));
    }

    ///// Setters /////
    public void SetLightExposition(Dictionary<Vector3, float> exposition)
    {
        lightExposition = exposition;
    }

    ///// Getters /////
    public float GetGlobalLightExposition()
    {
        if (lightExposition == null) return lightSeuilOfDeaph;
        float globalExposition = 0f;
        foreach (KeyValuePair<Vector3, float> lightDirection in lightExposition)
            globalExposition += lightDirection.Value;
        return globalExposition / lightExposition.Count;
    }
}
