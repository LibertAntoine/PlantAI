using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PlantAI;

public class BranchMotor : MonoBehaviour
{
    private Dictionary<Vector3, float> lightExposition;
    private Dictionary<Vector3, Vector3> skeletonPoints = new Dictionary<Vector3, Vector3>();
    private List<uint> alreadySelectedDirections = new List<uint>();
    private List<int> alreadySelectedSkeletonPoints = new List<int>();

    private BranchGrowMotor branchGrowMotor;
    private BranchColorMotor branchColorMotor;
    private BranchAnimator branchAnimator;

    void Awake()
    {
        if (!gameObject.GetComponent<BranchMotor>())
            Debug.LogError("GameObject '" + name + "' should have BranchGrowMotor script to manage grow expansion.");
        else
            branchGrowMotor = gameObject.GetComponent<BranchGrowMotor>();

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
        //
        CreateNewChildBranch();
        StartCoroutine(NewBranch());
    }
    

    void Update()
    {

        if (branchGrowMotor)
           branchGrowMotor.Grow(0.0001f);

        if (branchColorMotor)
            branchColorMotor.UpdateColor();

        if (branchAnimator)
            branchAnimator.UpdateAnimation(GetGlobalLightExposition() / 100000f + 0.1f);

        //Debug.Log(GetGlobalLightExposition());
        // if light enough
        // new branch on side
        // 

    }

    public void CreateNewChildBranch()
    {
        if (lightExposition == null)
        {
            CreateNewChildBranch(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)));
        } else {
            uint index = 0;
            uint selectedDirection = 0;
            KeyValuePair<Vector3, float> mostExposedDirection;
            foreach (KeyValuePair<Vector3, float> lightDirection in lightExposition) {
                if (mostExposedDirection.Value < lightDirection.Value && !alreadySelectedDirections.Contains(index))
                {
                    selectedDirection = index;
                    mostExposedDirection = lightDirection;
                }
                index++;
            }

            alreadySelectedDirections.Add(selectedDirection);
            Debug.Log(mostExposedDirection.Key);
            CreateNewChildBranch(mostExposedDirection.Key);
        }
    }

    public void CreateNewChildBranch(Vector3 direction)
    {
        uint tried = 0;
        int index = 0;
        while(alreadySelectedSkeletonPoints.Contains(index) && tried < skeletonPoints.Count)
        {
            index = Random.Range(0, skeletonPoints.Count);
            tried++;
        }
            

        alreadySelectedSkeletonPoints.Add(index);

        CreateNewChildBranch(direction, skeletonPoints.ElementAt(index));
    }

    public void CreateNewChildBranch(Vector3 direction, KeyValuePair<Vector3, Vector3> positionAndNormal)
    {
        Vector3 RandomInfluenceDirection = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0.2f, 0.8f), Random.Range(-0.1f, 0.1f));

        Quaternion quat = Quaternion.FromToRotation(positionAndNormal.Value, direction + RandomInfluenceDirection);
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

    ///// Getters /////
    public float GetGlobalLightExposition()
    {
        if(lightExposition == null) return 100000f;
        float globalExposition = 0f;
        foreach (KeyValuePair<Vector3, float> lightDirection in lightExposition)
            globalExposition += lightDirection.Value;
        return globalExposition / lightExposition.Count;
    }
}
