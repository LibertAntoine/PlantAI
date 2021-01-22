using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BranchMotor : MonoBehaviour
{
    private Dictionary<Vector3, float> lightExposition;
    private Dictionary<Vector3, Vector3> skeletonPoints = new Dictionary<Vector3, Vector3>();

    private BranchGrowMotor branchGrowMotor;

    void Start()
    {
        if (!gameObject.GetComponent<BranchMotor>())
            Debug.LogError("GameObject '" + name + "' should have BranchGrowMotor script to manage grow expansion.");
        else
            branchGrowMotor = gameObject.GetComponent<BranchGrowMotor>();


        StartCoroutine(NewBranch());
    }

    private IEnumerator NewBranch()
    {
        yield return new WaitForSeconds(7);
        CreateNewChildBranch(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)));

    }
    

    void Update()
    {
        //Debug.Log(lightExposition.ElementAt(1).Value);


        if (branchGrowMotor)
           branchGrowMotor.Grow(0.0001f);

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

    public void AddSkeletonPoint(Vector3 position, Vector3 normal)
    {
        skeletonPoints.Add(position, normal);
    }

    ///// Setters /////
    public void SetLightExposition(Dictionary<Vector3, float> exposition)
    {
        lightExposition = exposition;
    }
}
