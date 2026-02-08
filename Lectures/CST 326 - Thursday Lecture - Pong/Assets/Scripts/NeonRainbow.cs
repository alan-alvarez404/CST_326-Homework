using UnityEngine;

public class NeonRainbowCycle : MonoBehaviour
{
    private Renderer targetRenderer;

    // Can change speed at which the colors cycle or brightness
    public float cycleSpeed = 1f;
    public float emissionIntensity = 2.5f;

    public bool randomizeStartingColor = true;
    
    public Material material;
    private float colorOffset;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        if (targetRenderer == null)
        {
            Debug.LogWarning("NeonRainbowCycle: No Renderer found.");
            return;
        }

        material = targetRenderer.material;

        colorOffset = 0f;
        if (randomizeStartingColor)
        {
            colorOffset = Random.Range(0f, 1f);
        }
    }

    private void Update()
    {
        if (material == null)
        {
            return;
        }

        float h = Mathf.Repeat(colorOffset + (Time.time * cycleSpeed), 1f);
        Color c = Color.HSVToRGB(h, 1f, 1f);

        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", c * emissionIntensity);

        material.color = c;
    }
}