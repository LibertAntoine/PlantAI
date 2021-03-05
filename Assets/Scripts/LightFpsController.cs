using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFpsController : MonoBehaviour
{
    float intensity = 0f;
    float scale = 0.25f;
    Light spotLight;

    void Awake()
    {
        spotLight = GetComponent<Light>();
        spotLight.intensity = intensity;
    }

    void Update()
    {
        // Compute the intensity value.
        intensity += Input.mouseScrollDelta.y * scale;
        intensity = Mathf.Clamp(intensity, 0f, 10f);

        // Assign it to the spotlight.
        spotLight.intensity = intensity;
    }
}