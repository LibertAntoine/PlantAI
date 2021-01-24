using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PlantAI;

public class BranchMotor : MonoBehaviour
{
    private Dictionary<Vector3, float> lightExposition;
    private Dictionary<Vector3, Vector3> skeletonPoints = new Dictionary<Vector3, Vector3>();

    private BranchColorMotor branchColorMotor;
    private BranchAnimator branchAnimator;

    void Awake()
    {
        if (!gameObject.GetComponent<BranchColorMotor>())
            Debug.LogError("GameObject '" + name + "' should have BranchColorMotor script to manage grow expansion.");
        else
            branchColorMotor = gameObject.GetComponent<BranchColorMotor>();

        if (!gameObject.GetComponent<BranchAnimator>())
            Debug.LogError("GameObject '" + name + "' should have BranchColorMotor script to manage grow expansion.");
        else
            branchAnimator = gameObject.GetComponent<BranchAnimator>();
    }

    void Start()
    {
        StartCoroutine(NewBranch());
    }

    private IEnumerator NewBranch()
    {
        yield return new WaitForSeconds(7);
        //CreateNewChildBranch(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)));
        CreateNewChildBranch();
        StartCoroutine(NewBranch());
    }

    void Update()
    {
        if (branchColorMotor)
            branchColorMotor.UpdateColor();

        if (branchAnimator)
        {
            branchAnimator.UpdateAnimation(GetGlobalLightExposition() / 100000f + 0.1f);
            branchAnimator.Grow(0.0001f);
        }

        Debug.Log(GetGlobalLightExposition());
        // if light enough
        // new branch on side
        // 

    }

    public void CreateNewChildBranch()
    {
        KeyValuePair<Vector3, float> mostExposedDirection;
        foreach (KeyValuePair<Vector3, float> lightDirection in lightExposition)
            if (mostExposedDirection.Value < lightDirection.Value)
                mostExposedDirection = lightDirection;

        CreateNewChildBranch(mostExposedDirection.Key);
    }

    public void CreateNewChildBranch(Vector3 direction)
    {
        CreateNewChildBranch(direction, skeletonPoints.ElementAt(Random.Range(0, skeletonPoints.Count)));
    }

    public void CreateNewChildBranch(Vector3 direction, KeyValuePair<Vector3, Vector3> positionAndNormal)
    {
        Quaternion quat = Quaternion.FromToRotation(positionAndNormal.Value, direction + new Vector3(0, 0.5f, 0));
        GameObject branch = (GameObject)Resources.Load("Prefabs/Branch", typeof(GameObject));
        Instantiate(branch, positionAndNormal.Key, quat, transform.parent);
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
        GameObject branch = Instantiate(branchPrefab, positionAndNormal.Key, quat, transform.parent) as GameObject;
        // Immediately set its radius.
        branch.GetComponent<BranchAnimator>().radius = radius;
    }

    public void AddSkeletonPoint(Vector3 position, Vector3 normal)
    {
        skeletonPoints.Add(position, normal);
    }

    ///// Setters /////
    public void SetLightExposition(Dictionary<Vector3, float> exposition)
    {
        lightExposition = exposition;
    }

    ///// Getters /////
    public float GetGlobalLightExposition()
    {
        if (lightExposition == null) return 100000f;
        float globalExposition = 0f;
        foreach (KeyValuePair<Vector3, float> lightDirection in lightExposition)
            globalExposition += lightDirection.Value;
        return globalExposition / lightExposition.Count;
    }
}
