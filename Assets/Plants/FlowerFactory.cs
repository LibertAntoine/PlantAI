using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlantAI
{
    public class FlowerFactory : MonoBehaviour
    {
        // ==============================
        // ATTRIBUTES
        // ==============================

        private string branchPrefabPath = "Prefabs/Flower";

        // ==============================
        // PUBLIC METHODS
        // ==============================

        /// <summary>
        /// Create new flower at the top of a branch
        /// </summary>
        /// <param name="positionAndNormal">Position (Vector3) and Normal (Vector3).</param>
        public void CreateNewFlower(KeyValuePair<Vector3, Vector3> positionAndNormal)
        {
            Instantiate(
                (GameObject)Resources.Load(branchPrefabPath, typeof(GameObject)),
                positionAndNormal.Key,
                Quaternion.FromToRotation(Vector3.up, positionAndNormal.Value),
                transform);
        }
    }
}
