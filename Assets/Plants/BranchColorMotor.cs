using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlantAI;

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
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_Color", Color.Lerp(Color1, Color2, Mathf.Min(branchAnimator.GetSliceRadius(0), 0.2f)));
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }
}
