using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlantAI
{
    public class BranchCreatorMotor : MonoBehaviour
    {
        // ==============================
        // ATTRIBUTES
        // ==============================

        /// <summary>Number max for génération (before creation of a flower).</summary>
        public int maxGeneration = 4;
        /// <summary>Generation management</summary>
        public int numberBranchesPerNewGroup = 3;

        /// <summary>Path of prefab for new branch instantiation.</summary>
        public string branchPrefabPath = "Prefabs/Branch";

        /// <summary>Generation of current branch.</summary>
        public uint generation;

        /// <summary>Script to create flower.</summary>
        private FlowerFactory flowerFactory;



        // ==============================
        // UNITY METHODS
        // ==============================

        void Awake()
        {
            generation = (transform.parent.GetComponent<BranchCreatorMotor>() != null) ? transform.parent.GetComponent<BranchCreatorMotor>().generation + 1 : 0;
            flowerFactory = FindObjectOfType<FlowerFactory>();
            if (flowerFactory == null) Debug.LogError("GameObject '" + name + "' don't find FlowerFactory and cannot create flower.");
        }


        // ==============================
        // PUBLIC METHODS
        // ==============================

        /// <summary>
        /// Create new group of child branch
        /// around the given skeleton point of the parent branch.
        /// </summary>
        /// <param name="positionAndNormal">Position (Vector3) and Normal (Vector3).</param>
        public void CreateNewChildBranch(KeyValuePair<Vector3, Vector3> positionAndNormal)
        {

            if (!lastGen() && !isNearObstacle(positionAndNormal.Key))
            {
                float offset = Random.Range(0, 2 * Mathf.PI);
                for (int i = 0; i < numberBranchesPerNewGroup; i++)
                {
                    float rotation = (2 * Mathf.PI / numberBranchesPerNewGroup) * i + offset;
                    Instantiate(
                        (GameObject)Resources.Load(branchPrefabPath, typeof(GameObject)),
                        positionAndNormal.Key, Quaternion.identity, transform)
                        .transform.localRotation = Quaternion.Euler(Mathf.Sin(rotation) * Mathf.Rad2Deg, 0, Mathf.Cos(rotation) * Mathf.Rad2Deg);
                }
            }
        }

        /// <summary>
        /// Create a branch in the continuity of
        /// the given position and normal.
        /// </summary>
        /// <param name="positionAndNormal">Position (Vector3) and Normal (Vector3).</param>
        /// <param name="radius">Base radius of the branch.</param>d
        public void CreateNewChildInContinuity(KeyValuePair<Vector3, Vector3> positionAndNormal, float radius)
        {
            if (!lastGen() && !isNearObstacle(positionAndNormal.Key))
            {
                var branch = Instantiate(
                    (GameObject)Resources.Load(branchPrefabPath, typeof(GameObject)),
                    positionAndNormal.Key,
                    Quaternion.FromToRotation(Vector3.up, transform.rotation * positionAndNormal.Value),
                    transform
                );
                branch.GetComponent<BranchAnimator>().radius = radius;
            }
            else if (flowerFactory != null)
            {
                flowerFactory.CreateNewFlower(positionAndNormal);
            }
        }

        // ==============================
        // PRIVATE METHODS
        // ==============================

        private bool lastGen()
        {
            return generation + 1 == maxGeneration;
        }

        private bool isNearObstacle(Vector3 position)
        {
            foreach (var obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
            {
                if(obstacle.GetComponent<Collider>().bounds.Contains(position))
                {
                    return true;
                }
            }
            return false;
        }

    }
}