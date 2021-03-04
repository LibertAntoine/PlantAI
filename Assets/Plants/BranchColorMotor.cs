using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlantAI
{
    public class BranchColorMotor : MonoBehaviour
    {
        public Color Color1, Color2;

        private Renderer _renderer;
        private MaterialPropertyBlock _propBlock;
        private BranchAnimator branchAnimator;

        void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
            branchAnimator = GetComponent<BranchAnimator>();
        }

        public void UpdateColor()
        {
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_Color", Color.Lerp(Color1, Color2, branchAnimator.GetSliceRadius(0) * 5f));
            _renderer.SetPropertyBlock(_propBlock);
        }
    }
}
