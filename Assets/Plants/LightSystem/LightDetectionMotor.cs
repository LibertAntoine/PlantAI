using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlantAI
{
    /// <summary>
    /// Magage the light detection system which give at each branch its light exposition in different direction
    /// This script need camera object (camera) and a texture renderer (lightCheckTexture) in which the camera is renderer
    /// All the branch visited should have a branchMotor script to receive its exposition datas.
    /// </summary>
    public class LightDetectionMotor : MonoBehaviour
    {
        public Camera lightCamera; // Camera use to scan branch
        public GameObject plantsGroup; // Parent object of all plants/branches
        public RenderTexture lightCheckTexture; // Texture Renderer in which is send the camera view
        public int picturePerBranch; // Number of picture take for scanning one branch

        private float rotation;
        private GameObject currentPlant;
        private Vector3 currentCamDirection;
        private float currentLightLevel;

        private bool debug = false; // Debug Mode

        enum Layer
        {
            Default = 0,
            LightDetection = 8
        }

        void Start()
        {
            currentPlant = plantsGroup;
            StartCoroutine(ScanBranch());
        }

        private IEnumerator ScanBranch()
        {
            Dictionary<Vector3, float> lightExposition = new Dictionary<Vector3, float>(picturePerBranch);
            yield return StartCoroutine(NextPlant()); // Go to next branch

            if (debug) Debug.Log(currentPlant); // Debug

            rotation = 0;
            while (rotation < 2.0f * Mathf.PI)
            {
                rotation += 2.0f * Mathf.PI / picturePerBranch; // Rotate camera angle
                yield return StartCoroutine(MoveCamera());
                yield return StartCoroutine(LightShot());

                if (debug) Debug.Log(currentLightLevel); // Debug
                lightExposition.Add(currentCamDirection, currentLightLevel);
            }

            BranchMotor branchMotor = currentPlant.GetComponent<BranchMotor>();
            if (branchMotor) branchMotor.SetLightExposition(lightExposition);
            else Debug.LogError("GameObject '" + currentPlant.name + "' should have BranchMotor script to manage light exposition.");

            StartCoroutine(ScanBranch());
        }

        /// Get the next plant element in the graph and place it in currentPlant
        private IEnumerator NextPlant()
        {
            currentPlant.layer = (int)Layer.Default;

            // Define next branch to visit
            if (currentPlant.transform.childCount != 0)
            {
                currentPlant = currentPlant.transform.GetChild(0).gameObject; // Go to child
            }
            else
            {
                while (currentPlant != plantsGroup && currentPlant.transform.parent.childCount <= (currentPlant.transform.GetSiblingIndex() + 1))
                {

                    currentPlant = currentPlant.transform.parent.gameObject; // Go back to parent
                }
                if (currentPlant == plantsGroup) currentPlant = currentPlant.transform.GetChild(0).gameObject; // Next Loop
                else currentPlant = currentPlant.transform.parent.GetChild(currentPlant.transform.GetSiblingIndex() + 1).gameObject; // Go to brother
            }

            // Change the layer of the next visited branch to be visible for the lightDetection Camera
            currentPlant.layer = (int)Layer.LightDetection;
            yield return new WaitUntil(() => { return currentPlant.layer == (int)Layer.LightDetection; }); // Layer change takes some times.
        }

        /// Take a shot of the light level in front of the camera.
        /// Get light level in currentLightLevel
        private IEnumerator LightShot()
        {
            // Capture texture state
            RenderTexture tmpTexture = RenderTexture.GetTemporary(lightCheckTexture.width, lightCheckTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(lightCheckTexture, tmpTexture);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmpTexture;

            // Generate 2DTexture from the RenderTexture.
            Texture2D temp2DTexture = new Texture2D(lightCheckTexture.width, lightCheckTexture.height);
            temp2DTexture.ReadPixels(new Rect(0, 0, tmpTexture.width, tmpTexture.height), 0, 0);
            temp2DTexture.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmpTexture);

            // Get pixel datas from the 2DTexture
            Color32[] colors = temp2DTexture.GetPixels32();

            currentLightLevel = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                // From colors to grey level
                currentLightLevel += (0.2126f * colors[i].r) + (0.0722f * colors[i].g) + (0.0722f * colors[i].b);
            }

            yield return new WaitForSeconds(0f);
        }

        /// Command orbital rotation of the camera around the branch
        /// Get camera direction vector in currentCamDirection
        private IEnumerator MoveCamera()
        {
            // Camera Translation
            float ray = 2 + currentPlant.transform.localScale.x;
            Vector3 rotationVector = new Vector3(Mathf.Sin(rotation) * ray, 0, Mathf.Cos(rotation) * ray);
            lightCamera.transform.position = currentPlant.transform.TransformPoint(currentPlant.GetComponent<MeshFilter>().sharedMesh.bounds.center + rotationVector);

            // Camera Rotation
            lightCamera.transform.rotation = currentPlant.transform.rotation;
            lightCamera.transform.Rotate(new Vector3(0, (rotation + Mathf.PI) * Mathf.Rad2Deg, 0));

            // Camera viewport adaptation
            float branchLength = currentPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.y * currentPlant.transform.localScale.y * 1.5f;
            lightCamera.orthographicSize = branchLength / 2.0f;

            float branchDiameter = Mathf.Max(currentPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * currentPlant.transform.localScale.x, currentPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * currentPlant.transform.localScale.z);
            lightCamera.rect = new Rect(1 - (branchDiameter / branchLength), 0.0f, 1.0f, 1.0f);

            currentCamDirection = new Vector3(Mathf.Sin(rotation), 0, Mathf.Cos(rotation));

            yield return new WaitForSeconds(0f);
        }
    }
}
