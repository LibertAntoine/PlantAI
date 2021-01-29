using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlantAI
{
    public class LeafFactory : MonoBehaviour
    {
        // ==============================
        // ATTRIBUTES
        // ==============================

        private string leafPrefabPath = "Prefabs/Leaf";

        // ==============================
        // PUBLIC METHODS
        // ==============================

        /// <summary>
        /// Create new leaf on a branch
        /// position depending of the given position and the given direction
        /// </summary>
        /// <param name="direction">Direction (Vector3) for oriented the leaf</param>
        /// <param name="positionAndNormal">Position (Vector3) and Normal (Vector3).</param>
        public void CreateNewLeaf(Vector3 direction, KeyValuePair<Vector3, Vector3> positionAndNormal)
        {
            Instantiate(
                (GameObject)Resources.Load(leafPrefabPath, typeof(GameObject)),
                positionAndNormal.Key,
                Quaternion.FromToRotation(Vector3.up, positionAndNormal.Value) * Quaternion.FromToRotation(positionAndNormal.Value, transform.InverseTransformDirection(direction)),
                transform);
        }
    }
}
