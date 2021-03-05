using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PlantAI
{
    public class BranchMotor : MonoBehaviour
    {


        public float lightSeuilOfDeath = 15000f;
        public float energieNeedForGrow = 200000f;
        public float energieForNewBranch = 70000000f;
        public int leafStyle = 0;

        /// <summary>Light information recup from LightDetectionMotor</summary>
        private Dictionary<Vector3, float> lightExposition;

        /// <summary>Regular skeleton points along </summary>
        private Dictionary<Vector3, Vector3> skeletonPoints = new Dictionary<Vector3, Vector3>();

        /// <summary>Branch script dependancies</summary>
        private BranchColorMotor branchColorMotor;
        private BranchAnimator branchAnimator;
        private BranchCreatorMotor branchCreatorMotor;
        private LeafFactory leafFactory;

        private bool haveChilds = false;
        private float growDelai = 20;

        private float accumulatedEnergie = 0;
        private float growFactor = 0;

        void Awake()
        {
            // Recup script dependencies
            branchColorMotor = gameObject.GetComponent<BranchColorMotor>();
            if (branchColorMotor == null) Debug.LogError("GameObject '" + name + "' should have BranchColorMotor script to manage the color of the branch.");

            branchAnimator = gameObject.GetComponent<BranchAnimator>();
            if (branchAnimator == null) Debug.LogError("GameObject '" + name + "' should have BranchAnimator script to manage grow expansion.");

            branchCreatorMotor = gameObject.GetComponent<BranchCreatorMotor>();
            if (branchCreatorMotor == null) Debug.LogError("GameObject '" + name + "' should have BranchCreatorMotor script to manage new branch creation.");

            leafFactory = FindObjectOfType<LeafFactory>();
            if (leafFactory == null) Debug.LogError("GameObject '" + name + "' don't find LeafFactory and cannot create leaf.");

        }

        private void Start()
        {
            if (branchAnimator)
                StartCoroutine(growPass());
        }


        void Update()
        {
            float globalExposition = GetGlobalLightExposition();
            Debug.Log(globalExposition);
            growFactor = (globalExposition - lightSeuilOfDeath) / energieNeedForGrow;

            if (branchAnimator)
                branchAnimator.UpdateAnimation(growFactor);
            

            if (!haveChilds) {
                accumulatedEnergie += Mathf.Max(globalExposition - lightSeuilOfDeath, 0);
                if (accumulatedEnergie > energieForNewBranch)
                {
                    haveChilds = true;
                    branchCreatorMotor.CreateNewChildBranch(giveRandomSkeletonPoint());
                }
            }

        }

        private IEnumerator growPass()
        {
            yield return new WaitForSeconds(growDelai);
            branchAnimator.Grow(0.0001f * (growFactor * 10 * growDelai / (branchCreatorMotor.generation + 1)));
            if (branchColorMotor)
                branchColorMotor.UpdateColor();
            StartCoroutine(growPass());
        }


        public void AddSkeletonPoint(Vector3 position, Vector3 normal)
        {
            normal = transform.rotation * normal;
            skeletonPoints.Add(position, normal);
            if (leafFactory != null) leafFactory.CreateNewLeaf(giveMostExposedDirection(), new KeyValuePair<Vector3, Vector3>(position, normal), leafStyle);
        }

        private KeyValuePair<Vector3, Vector3> giveRandomSkeletonPoint()
        {
            return skeletonPoints.ElementAt(Random.Range(0, skeletonPoints.Count - 1));
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
}
