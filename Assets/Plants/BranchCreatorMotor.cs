using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlantAI;

public class BranchCreatorMotor : MonoBehaviour
{

    /// <summary>Genration management</summary>
    public int numberMaxGeneration = 2;
    private int numberBranchesPerNewGroup = 3;

    private uint generation;
    
    void Awake()
    {
        generation = (transform.parent.GetComponent<BranchCreatorMotor>() != null) ? transform.parent.GetComponent<BranchCreatorMotor>().generation + 1 : 0;
    }

    /// <summary>
    /// Create new group of child branch
    /// around the given skeleton point of the parent branch.
    /// </summary>
    /// <param name="positionAndNormal">Position (Vector3) and Normal (Vector3).</param>
    public void CreateNewChildBranch(KeyValuePair<Vector3, Vector3> positionAndNormal)
    {
        if (!lastGen())
        {
            for (int i = 0; i < numberBranchesPerNewGroup; i++)
            {
                float rotation = (2 * Mathf.PI / numberBranchesPerNewGroup) * i;
                Instantiate(
                    (GameObject)Resources.Load("Prefabs/Branch", typeof(GameObject)), 
                    positionAndNormal.Key, Quaternion.identity, transform)
                    .transform.localRotation = Quaternion.Euler(Mathf.Sin(rotation) * Mathf.Rad2Deg, 0, Mathf.Cos(rotation) * Mathf.Rad2Deg);
            }
        }
        else
        {
            Quaternion quat = Quaternion.FromToRotation(Vector3.up, positionAndNormal.Value);
            Instantiate((GameObject)Resources.Load("Prefabs/Flower", typeof(GameObject)), positionAndNormal.Key, quat, GameObject.FindGameObjectsWithTag("ParentLeaf")[0].transform);
        }
    }

    /// <summary>
    /// Create a branch in the continuity of
    /// the given position and normal.
    /// </summary>
    /// <param name="positionAndNormal">Position (Vector3) and Normal (Vector3).</param>
    /// <param name="radius">Base radius of the branch.</param>
    public void CreateNewChildInContinuity(KeyValuePair<Vector3, Vector3> positionAndNormal, float radius)
    {
        if (!lastGen())
        {
            Instantiate(
                (GameObject)Resources.Load("Prefabs/Branch", typeof(GameObject)), 
                positionAndNormal.Key,
                 Quaternion.FromToRotation(Vector3.up, transform.rotation * positionAndNormal.Value), 
                transform)
                    .GetComponent<BranchAnimator>().radius = radius;
        }
        else
        {
   
            Quaternion quat = Quaternion.FromToRotation(Vector3.up, positionAndNormal.Value);
            Instantiate((GameObject)Resources.Load("Prefabs/Flower", typeof(GameObject)), positionAndNormal.Key, quat, GameObject.FindGameObjectsWithTag("ParentLeaf")[0].transform);
        }
    }

    private bool lastGen() {
        return generation + 1 == numberMaxGeneration;
    }


}
