using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchColorMotor : MonoBehaviour
{
    public Color Color1, Color2;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    public void UpdateColor()
    {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_Color", Color.Lerp(Color1, Color2, Mathf.Min(transform.localScale.x, 1)));
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }
}
