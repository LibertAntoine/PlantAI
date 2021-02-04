using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlantAI
{
    public class BranchColorMotor : MonoBehaviour
    {
        public Color Color1, Color2;

        private Renderer _renderer;
        private MaterialPropertyBlock propBlock;
        private BranchAnimator branchAnimator;

        void Awake()
        {
            propBlock = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
            branchAnimator = GetComponent<BranchAnimator>();
        }

        public void UpdateColor()
        {
            _renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", Color.Lerp(Color1, Color2, Mathf.Min(branchAnimator.GetSliceRadius(0), 0.1f)));
        }
    }
}
