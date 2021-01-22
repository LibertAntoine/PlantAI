using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace PlantAI
{

    /// <summary>
    /// Class used to extrude branch mesh and
    /// create its animation through time.
    /// </summary>
    public class BranchAnimator : MonoBehaviour
    {
        // ==============================
        // ATTRIBUTES
        // ==============================

        /// <summary>Factor of speed progession of the plant growth</summary>
        public float growSpeedFactor = 0.3f;
        /// <summary>Second between to extrusions.</summary>
        public int timeIntoExtrusion = 5;
       
        /// <summary>Number of the current frame in the animation.</summary>
        int currentAnimationFrame = 0;
        /// <summary>Direction of the branch for the animation.</summary>
        Vector3 direction = Vector3.up;

        /// <summary>PB Mesh used to deform the Unity Mesh.</summary>
        ProBuilderMesh mesh;
        /// <summary>Faces to extrude each time. (it never changes)</summary>
        List<Face> extrudable = new List<Face>();
        /// <summary>Center shared vertex of the faces to extrude. (it never changes)</summary>
        SharedVertex centerExtrudableVertex;
        /// <summary>Indices of shared vertex to animate.</summary>
        List<int> sharedIndicesToAnimate = new List<int>();
        /// <summary>Indices of raw vertex to animate.</summary>
        List<int> rawIndicesToAnimate = new List<int>();

        /// <summary>Flag for running.</summary>
        bool running = true;

        // ==============================
        // UNITY METHODS
        // ==============================

        void Start()
        {
            // Retrieve the PB Mesh.
            mesh = gameObject.GetComponent<ProBuilderMesh>();
            // Setup faces and indices attributes.
            SetupExtrudableFaces();
            // Extrude the mesh once.
            Extrude();
        }

        void Update()
        {
            if (!running)
            {
                return;
            }

            if (currentAnimationFrame < timeIntoExtrusion * 60)
            {
                mesh.TranslateVertices(rawIndicesToAnimate, direction * Time.deltaTime * growSpeedFactor);
                mesh.Refresh();

                ++currentAnimationFrame;
                return;
            }

            // Reset current animation frame.
            currentAnimationFrame = 0;

            if (remainingExtrusions > 0)
            {
                Extrude();
                return;
            }

            // Stop running the script if max number extrusion reached.
            running = false;
        }

        // ==============================
        // PRIVATE METHODS
        // ==============================

        #region Utils

        /// <summary>
        /// Get indices of all the raw vertices
        /// corresponding to one SharedVertex.
        /// </summary>
        /// <param name="sharedIndex">Index of the SharedVertex.</param>
        /// <returns>Array of raw indices.</returns>
        int[] GetRawIndicesFromSharedIndex(int sharedIndex)
        {
            var sharedVertex = mesh.sharedVertices[sharedIndex];
            return GetRawIndicesFromSharedVertex(sharedVertex);
        }

        /// <summary>
        /// Get indices of all the raw vertices
        /// corresponding to one SharedVertex.
        /// </summary>
        /// <param name="sharedVertex">Reference to a SharedVertex.</param>
        /// <returns>Array of raw indices.</returns>
        int[] GetRawIndicesFromSharedVertex(SharedVertex sharedVertex)
        {
            var indices = new List<int>(sharedVertex);
            return indices.ToArray();
        }

        /// <summary>
        /// Get index of the SharedVertex
        /// corresponding to a raw vertex.
        /// </summary>
        /// <param name="rawIndex">Index of a raw vertex.</param>
        /// <returns>
        /// Index of the associated SharedVertex.
        /// Returns -1 if there is no SharedVertex.
        /// </returns>
        int GetSharedIndexFromRawIndex(int rawIndex)
        {
            for (var i = 0; i < mesh.sharedVertices.Count; ++i)
            {
                var sharedVertex = mesh.sharedVertices[i];

                if (sharedVertex.Contains(rawIndex)) { return i; }
            }

            return -1;
        }

        /// <summary>
        /// Get the position of a SharedVertex.
        /// </summary>
        /// <param name="sharedVertex">Reference to a SharedVertex.</param>
        /// <returns>Position of the SharedVertex. Coord space to determine.</returns>
        Vector3 GetSharedVertexPosition(SharedVertex sharedVertex)
        {
            var v = mesh.GetVertices()[sharedVertex[0]];
            return v.position;
        }

        #endregion

        #region Getters

        /// <summary>
        /// Get the position of the extrudable face's center vertex.
        /// </summary>
        /// <returns>Position of the vertex.</returns>
        Vector3 GetCenterExtrudablePosition()
        {
            return GetSharedVertexPosition(centerExtrudableVertex);
        }

        /// <summary>
        /// Get the normal of the extrudable face's center vertex.
        /// </summary>
        /// <returns>Normal of the vertex.</returns>
        Vector3 GetCenterExtrudableNormal()
        {
            var v = mesh.GetVertices()[centerExtrudableVertex[0]];
            return v.normal;
        }

        #endregion

        #region SetupMethods

        /// <summary>
        /// Setup the extrudable faces' attributes.
        /// </summary>
        void SetupExtrudableFaces()
        {
            // Make sure to empty the extrudable list.
            extrudable.Clear();

            // Get all the raw vertices.
            var allVerts = mesh.GetVertices();
            // Define the position where the centerExtrudableVertex should be.
            var centerExtrudablePosition = new Vector3(0, 0.5f, 0);

            // Set the centerExtrudableVertex by comparing sharedVertices positions.
            for (var i = 0; i < mesh.sharedVertices.Count; ++i)
            {
                var sharedVertex = mesh.sharedVertices[i];
                if ((GetSharedVertexPosition(sharedVertex) - centerExtrudablePosition).sqrMagnitude < 0.01f)
                {
                    centerExtrudableVertex = sharedVertex;
                    break;
                }
            }

            if (centerExtrudableVertex == null)
            {
                throw new MissingReferenceException("centerExtrudableVertex is not set.");
            }

            // Move all the vertices to place the orbit point at the base.
            mesh.TranslateVertices(mesh.faces, centerExtrudablePosition);

            // Get the raw vertex indices corresponding to the center point of extrudable faces.
            var centerExtrudableVertexIndices = new List<int>(GetRawIndicesFromSharedVertex(centerExtrudableVertex));

            // Go through all the mesh faces to get only the extrudable ones.
            foreach (var face in mesh.faces)
            {
                bool toExtrude = false;
                var indices = face.distinctIndexes;

                // Set flag to add the face to 'extrudable' if it contains a vertex which
                // is considered as a coindicent of the centerExtrudableVertex.
                foreach (var index in indices)
                {
                    if (centerExtrudableVertexIndices.Contains(index))
                    {
                        toExtrude = true;
                        break;
                    }
                }

                if (toExtrude) { extrudable.Add(face); }
            }
        }

        /// <summary>
        /// Setup the extrudable vertex indices attributes.
        /// </summary>
        void SetupIndicesToAnimate()
        {
            // Make sure to empty the lists.
            sharedIndicesToAnimate.Clear();
            rawIndicesToAnimate.Clear();

            // Populate rawIndicesToAnimate and remove doubles if there are some.
            foreach (var face in extrudable)
            {
                rawIndicesToAnimate.AddRange(face.distinctIndexes);
            }
            rawIndicesToAnimate = new List<int>(new HashSet<int>(rawIndicesToAnimate));

            // Populate sharedIndicesToAnimate without doubles (possible because not using AddRange).
            var _sharedIndicesToAnimate = new HashSet<int>();
            foreach (var i in rawIndicesToAnimate)
            {
                _sharedIndicesToAnimate.Add(GetSharedIndexFromRawIndex(i));
            }
            sharedIndicesToAnimate = new List<int>(_sharedIndicesToAnimate);
        }

        #endregion

        void Extrude()
        {
            // Decrement remaining extrusions.
            --remainingExtrusions;

            // Extrude the mesh.
            var faces = mesh.Extrude(extrudable, ExtrudeMethod.FaceNormal, 0.05f);
            Smoothing.ApplySmoothingGroups(mesh, faces, 60);
            // Apply the geometry to the mesh. TODO: check if necessary.
            mesh.ToMesh();
            // Rebuild UVs, Collisions, Tangents, etc. TODO: check if necessary.
            mesh.Refresh();

            // Reset indices to animate.
            SetupIndicesToAnimate();

            // Compute a direction.
            float randomness = 0.2f;
            direction = GetCenterExtrudableNormal() +
                new Vector3(
                    Random.Range(-randomness, randomness),
                    Random.Range(-randomness, randomness),
                    Random.Range(-randomness, randomness)
                );
            direction.Normalize();

            // // Add point to the branch skeleton.
            // GetComponent<BranchMotor>()
            //     .AddSkeletonPoint(GetCenterExtrudablePosition(), direction);

            // Rotate the face.
            foreach (var i in sharedIndicesToAnimate)
            {
                Quaternion quat = Quaternion.FromToRotation(GetCenterExtrudableNormal(), direction);
                Vector3 currentPosition = GetSharedVertexPosition(mesh.sharedVertices[i]);
                Vector3 newPosition = quat * (currentPosition - GetCenterExtrudablePosition());
                Vector3 diff = newPosition + GetCenterExtrudablePosition() - currentPosition;

                mesh.TranslateVertices(GetRawIndicesFromSharedIndex(i), diff);
            }

  
        }
    }
}
