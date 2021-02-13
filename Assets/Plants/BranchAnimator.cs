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

        /// <summary>Factor of speed progession of the plant growth.</summary>
        [Tooltip("Factor of speed progession of the plant growth.")]
        public float growSpeedFactor = 0.15f;
        /// <summary>Number of remaining extrusions. Set value to the max number.</summary>
        [Tooltip("Number of remaining extrusions. Set value to the max number.")]
        public int remainingExtrusions = 10;
        /// <summary>Base radius of the branch.</summary>
        [Tooltip("Base radius of the branch.")]
        public float radius = 0.05f;
        /// <summary>
        /// Define the length of each slice extrusion. 
        /// Make it low to get a good precision (more energy-intensive).
        /// </summary>
        [Tooltip("Define the length of each slice extrusion. Make it low to get a good precision (more energy-intensive).")]
        public double croissanceLimitPerExtrude = 0.3;

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
        /// <summary>Shared indices of each vertex per extrude slice.</summary>
        List<List<int>> sliceIndices = new List<List<int>>();

        /// <summary>Tendency of the plant to go upwards</summary>
        float upInfluence = 0.3f;

        /// <summary>Flag for running.</summary>
        bool running = true;

        float croissance = 0;

        // ==============================
        // UNITY METHODS
        // ==============================

        void Start()
        {
            // Retrieve the PB Mesh.
            mesh = gameObject.GetComponent<ProBuilderMesh>();
            // Setup faces and indices attributes.
            SetupExtrudableFaces();
            // Setup slice indices.
            SetSliceIndices();
            // Make the branch very THIN.
            ShrinkExpand(-(0.5f - radius));
            // Extrude the mesh once.
            Extrude();
        }

        // ==============================
        // PUBLIC METHODS
        // ==============================

        public void UpdateAnimation(float energie)
        {

            if (!running || energie < 0)
            {
                return;
            }
           

            var offset = Time.deltaTime * growSpeedFactor * Mathf.Min(energie, 1f);
            croissance += offset;
            mesh.TranslateVertices(rawIndicesToAnimate, direction * offset);
            mesh.Refresh();


            if (croissance > croissanceLimitPerExtrude)
            {

                // Add point to the branch skeleton.
                //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //sphere.transform.position = transform.TransformPoint(GetCenterExtrudablePosition());

                GetComponent<BranchMotor>().AddSkeletonPoint(transform.TransformPoint(GetCenterExtrudablePosition()), direction);
                croissance = 0;

                if (remainingExtrusions > 0)
                {
                    Extrude();
                }
                else
                {

                    // Start a new branch in the continuity.
                    GetComponent<BranchCreatorMotor>().
                        CreateNewChildInContinuity(
                            new KeyValuePair<Vector3, Vector3>(
                                transform.TransformPoint(GetCenterExtrudablePosition()),
                                GetCenterExtrudableNormal()
                            ),
                            GetHeadSliceRadius()
                        );

                    // Stop running the script if max number extrusion reached.
                    running = false;
                }
            }
        }

        /// <summary>
        /// Make the branch become thicker.
        /// </summary>
        /// <param name="factor">Factor of the translation.</param>
        public void Grow(float factor = 0.01f)
        {
            if (transform.parent.GetComponent<BranchAnimator>() && GetSliceRadius(0) + factor > transform.parent.GetComponent<BranchAnimator>().GetSliceRadius(0))
            {
                return;
            }

            /*
            if (factor <= 0)
            {
                throw new System.Exception("BranchAnimator.Grow: 'factor' arg cannot be negative.");
            }*/

            ShrinkExpand(factor);
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

        /// <summary>
        /// Get the center position of one slice.
        /// </summary>
        /// <param name="index">Index of the slice.</param>
        /// <returns>Position of the center point.</returns>
        Vector3 GetCenterOfSlice(int index)
        {
            var slice = sliceIndices[index];

            // Get the center point of the slice.
            var centerPoint = new Vector3(0, 0, 0);
            foreach (var i in slice)
            {
                centerPoint += GetSharedVertexPosition(mesh.sharedVertices[i]);
            }
            centerPoint /= slice.Count;

            return centerPoint;
        }

        /// <summary>
        /// Get the radius of one slice.
        /// </summary>
        /// <param name="index">Index of the slice.</param>
        /// <returns>Radius of the slice.</returns>
        public float GetSliceRadius(int index)
        {
            var slice = sliceIndices[index];
            var indexOfOnePoint = slice[0];
            var center = GetCenterOfSlice(index);

            return Vector3.Distance(
                GetSharedVertexPosition(mesh.sharedVertices[indexOfOnePoint]),
                center
            );
        }

        /// <summary>
        /// Get the radius of the head slice.
        /// </summary>
        /// <returns>Radius of the head slice.</returns>
        float GetHeadSliceRadius()
        {
            return GetSliceRadius(sliceIndices.Count - 1);
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
                Debug.Log(centerExtrudablePosition);
                throw new MissingReferenceException("centerExtrudableVertex is not set.");
            }

            // Move all the vertices to place the orbit point at the base.
            mesh.TranslateVertices(mesh.faces, new Vector3(0, -0.3f, 0));

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

        /// <summary>
        /// Set the sliceIndices attribute.
        /// Allow to get every vertex of each slice.
        /// Useful to make the branch thicker, for instance.
        /// </summary>
        void SetSliceIndices()
        {
            int NB_VERTEX_PER_SLICE = 8;

            // Clear the slice indices list.
            sliceIndices.Clear();

            // Get the number of slices in the branch.
            // We do not take the two center faces vertices.
            int sliceCount = (mesh.sharedVertices.Count - 2) / NB_VERTEX_PER_SLICE;

            // Fill the list with lists of indices per slice.
            for (var i = 0; i < sliceCount; ++i)
            {
                var slice = new List<int>();
                for (var j = 0; j < NB_VERTEX_PER_SLICE; ++j)
                {
                    slice.Add(i + j * sliceCount);
                }
                sliceIndices.Add(slice);
            }
        }

        /// <summary>
        /// Extrude the branch and create a new slice.
        /// </summary>
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

            // Re-set slice indices.
            SetSliceIndices();

            // Reset indices to animate.
            SetupIndicesToAnimate();

            // Compute a direction.
            var direction = ComputeDirection();

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

        /// <summary>Compute an extrude direction.</summary>
        /// <returns>Direction.</returns>
        Vector3 ComputeDirection()
        {
            // Compute a new direction.
            float randomness = 0.4f;
            direction = GetCenterExtrudableNormal() +
                 transform.InverseTransformDirection(Vector3.up) * upInfluence +
                new Vector3(
                    Random.Range(-randomness, randomness),
                    Random.Range(-randomness, randomness),
                    Random.Range(-randomness, randomness)
                );
            direction.Normalize();

            // Check if the reaching point is accessible.
            foreach (var go in GameObject.FindGameObjectsWithTag("Obstacle"))
            {
                var reachPosition =
                    transform.TransformPoint(GetCenterExtrudablePosition()) +
                    direction * (float)croissanceLimitPerExtrude;

                var c = go.GetComponent<Collider>();

                // Check if the reachPoint is inside the Collider.
                if (c.bounds.Contains(reachPosition))
                {
                    // Get the closest point on the obstacle's bounds
                    // and compute a director vector from its center.
                    var closest = c.ClosestPointOnBounds(reachPosition);
                    var center = c.bounds.center;
                    var dir = closest - center;
                    // Make the director vector more random. Very useful because we cannot just
                    // take the director vector to the closest point if it is orthogonal to the Branch.
                    // This is a little workaround which can be improved.
                    dir += new Vector3(Random.Range(0, 0.5f), Random.Range(0, 0.5f), Random.Range(0, 0.5f));
                    dir.Normalize();
                    // Compute a position outside the obstacle where the Branch should go towards.
                    var outsidePoint = closest + dir * 5 * GetHeadSliceRadius();

                    // Set the new direction.
                    direction = outsidePoint - transform.TransformPoint(GetCenterExtrudablePosition());
                }
            }

            return direction;
        }

        /// <summary>
        /// Make the branch become thinner (negative factor)
        /// or thicker (positive factor).
        /// </summary>
        /// <param name="factor">Factor of the translation.</param>
        void ShrinkExpand(float factor)
        {
            for (var index = 0; index < sliceIndices.Count; ++index)
            {
                var slice = sliceIndices[index];
                var centerPoint = GetCenterOfSlice(index);

                // Make the translation for each vertex of the slice
                // towards/backwards the center point.
                foreach (var i in slice)
                {
                    var direction = GetSharedVertexPosition(mesh.sharedVertices[i]) - centerPoint;
                    direction.Normalize();
                    var rawIndices = GetRawIndicesFromSharedIndex(i);

                    mesh.TranslateVertices(rawIndices, direction * factor);
                }
            }
        }
    }
}
